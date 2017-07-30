using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    Char character;

    public GameObject hexUnitPrefab;
    public float chanceSource = 0.1f;
    public float chanceWall = 0.2f;
    public System.Action Pulse;

    Dictionary<string, HexUnit> grid;
    Dictionary<string, List<string>> paths;
    Dictionary<string, List<string>> incompletePaths;
    Dictionary<string, Source> sources;

    Vector2[] evenBorder;
    Vector2[] oddBorder;

    bool active;
    const float factor = 1.7320508076f;
    const float unitSize = 50f;

    float pulseInterval = 0.75f;
    float nextPulse;

    void Awake()
    {
        active = true;
        grid = new Dictionary<string, HexUnit>();
        paths = new Dictionary<string, List<string>>();
        incompletePaths = new Dictionary<string, List<string>>();
        sources = new Dictionary<string, Source>();

        evenBorder = new Vector2[6]
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(1, 0),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

        oddBorder = new Vector2[6]
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(1, 0),
            new Vector2(-1, 1),
            new Vector2(1, 1)
        };
    }

    void Start()
    {
        character = FindObjectOfType<Char>();
        nextPulse = Time.time;
    }

    void Update()
    {
        if (active && Time.time > nextPulse)
        {
            nextPulse += pulseInterval;
            if (Pulse != null) Pulse();
        }
    }

    public void CreateUnit(Vector2 coords)
    {
        if (coords == Vector2.zero) return;

        string key = coords.x + ":" + coords.y;
        if (grid.ContainsKey(key) && key != "0:0") return;

        bool checkAfterwards = false;
        Vector2 check = Vector2.zero;
        bool noSource = CheckSourceRestrictions(coords, out checkAfterwards, out check);

        GameObject unit = Instantiate(hexUnitPrefab);
        unit.transform.SetParent(transform);
        unit.transform.localPosition = ToHexCoords(coords);

        HexUnit hexUnit = unit.GetComponent<HexUnit>();
        hexUnit.coords = coords;
        HexUnit.Type type = noSource ? HexUnit.Type.Empty : SelectType();
        hexUnit.ChangeType(type);
        hexUnit.Rise();
        grid.Add(key, hexUnit);

        if (type == HexUnit.Type.Source)
        {
            hexUnit.ChangeStatus(HexUnit.State.Trail, coords);
            StartPath(coords);
        }
        if (checkAfterwards)
        {
            CheckPossibles(check, check);
        }

    }

    bool CheckSourceRestrictions(Vector2 coords, out bool checkAfterwards, out Vector2 source)
    {
        checkAfterwards = false;
        source = Vector2.zero;

        //No around base
        for (int i = 0; i < 6; i++)
        {
            if (coords == evenBorder[i])
            {
                return true;
            }
        }

        //No beside other source
        Vector2[] borders = coords.x % 2 == 0 ? evenBorder : oddBorder;

        for (int i = 0; i < 6; i++)
        {
            Vector2 current = coords + borders[i];
            HexUnit unit = GetUnit(current);
            if (unit != null && (unit.state != HexUnit.State.Free))
            {
                if (unit.state == HexUnit.State.Trail)
                {
                    checkAfterwards = true;
                    source = current;
                }
                return true;
            }
        }
        return false;
    }

    void StartPath(Vector2 source)
    {
        string key = source.x + ":" + source.y;
        List<string> path = new List<string>();
        path.Add(key);
        incompletePaths.Add(key, path);

        CheckPossibles(source, source);
    }

    public void PlaceCable(Vector2 cable, Vector2 source)
    {
        //Check if Char have resources
        if (!character.HaveResources())
            return;

        HexUnit unit = GetUnit(cable);
        unit.ChangeType(HexUnit.Type.Cable);
        unit.ChangeStatus(HexUnit.State.Trail, source);
        string key = source.x + ":" + source.y;
        List<string> path;
        incompletePaths.TryGetValue(key, out path);
        if (path != null)
        {
            string newPoint = cable.x + ":" + cable.y;
            string previous = path[path.Count - 1];
            path.Add(newPoint);
            incompletePaths.Remove(key);

            if (CheckPossibles(cable, source))
            {
                paths.Add(key, path);
                CompletePath(path);
            }
            else
            {
                incompletePaths.Add(key, path);
            }

            CleanUp(StringToVector(previous));
        }
    }

    void CompletePath(List<string> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            HexUnit unit = GetUnit(StringToVector(path[i]));
            if (unit != null)
            {
                unit.ChangeStatus(HexUnit.State.InUse);
            }
        }

        string key = path[0];
        Source s;
        sources.TryGetValue(key, out s);
        if (s != null)
        {
            s.StartExtract();
        }
    }

    public bool CheckPossibles(Vector2 coord, Vector2 source)
    {
        Vector2[] borders = coord.x % 2 == 0 ? evenBorder : oddBorder;

        for (int i = 0; i < 6; i++)
        {
            Vector2 current = coord + borders[i];
            if (current == Vector2.zero) return true;

            HexUnit unit = GetUnit(current);
            if (unit != null)
            {
                if (unit.state == HexUnit.State.InUse)
                {
                    return true;
                }

                if (unit.type == HexUnit.Type.Empty && unit.state == HexUnit.State.Free)
                {
                    unit.ChangeStatus(HexUnit.State.Possible, source);
                }
            }
        }

        return false;
    }

    void CleanUp(Vector2 coord)
    {
        HexUnit previous = GetUnit(coord);
        previous.ChangeStatus(HexUnit.State.Cable);

        Vector2[] borders = coord.x % 2 == 0 ? evenBorder : oddBorder;

        for (int i = 0; i < 6; i++)
        {
            Vector2 current = coord + borders[i];
            HexUnit unit = GetUnit(current);
            if (unit != null && unit.state == HexUnit.State.Possible)
            {
                unit.ChangeStatus(HexUnit.State.Free);
            }
        }
    }

    HexUnit GetUnit(Vector2 coords)
    {
        string key = coords.x + ":" + coords.y;
        HexUnit unit;
        grid.TryGetValue(key, out unit);
        return unit;
    }

    public void AddSource(Vector2 coords, Source s)
    {
        string key = coords.x + ":" + coords.y;
        sources.Add(key, s);
    }

    public Vector2 GetNextCable(Vector2 source, int i_cable)
    {
        string key = source.x + ":" + source.y;
        List<string> path;
        paths.TryGetValue(key, out path);
        if (path != null)
        {
            return StringToVector(path[i_cable]);
        }
        return Vector2.zero;
    }

    public static Vector3 ToHexCoords(Vector2 coords)
    {
        if (coords.x % 2 == 0)
            return new Vector3(coords.x * 1.5f * unitSize, 0f, coords.y * factor * unitSize);
        return new Vector3(coords.x * 1.5f * unitSize, 0f, (coords.y + 0.5f) * factor * unitSize);
    }

    Vector2 StringToVector(string coord)
    {
        string[] split = coord.Split(':');
        return new Vector2(int.Parse(split[0]), int.Parse(split[1]));
    }

    HexUnit.Type SelectType()
    {
        float chance = Random.value;

        if (chance > chanceWall + chanceSource) return HexUnit.Type.Empty;

        return (chance < chanceSource) ? HexUnit.Type.Source : HexUnit.Type.Wall;
    }
}
