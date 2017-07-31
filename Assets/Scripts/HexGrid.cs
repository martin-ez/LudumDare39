using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    Character character;

    public GameObject hexUnitPrefab;
    public float chanceSource = 0.1f;
    public float chanceWall = 0.4f;
    public float oreChance = 0.5f;
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

    float pulseInterval = 1.25f;
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
        character = FindObjectOfType<Character>();
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

        bool noSource = CheckSourceRestrictions(coords);

        GameObject unit = Instantiate(hexUnitPrefab);
        unit.transform.SetParent(transform);
        unit.transform.localPosition = ToHexCoords(coords);

        HexUnit hexUnit = unit.GetComponent<HexUnit>();
        hexUnit.coords = coords;
        float chance = Random.value;
        HexUnit.Type type = noSource ? (chance > 0.5f ?HexUnit.Type.Empty : HexUnit.Type.Ore) : SelectType();
        hexUnit.ChangeType(type);
        hexUnit.Rise();
        grid.Add(key, hexUnit);

        if (type == HexUnit.Type.Source)
        {
            hexUnit.ChangeStatus(HexUnit.State.Trail, coords);
            StartPath(coords);
        }

        CheckGridConnections();
    }

    bool CheckSourceRestrictions(Vector2 coords)
    {
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
            if (unit != null && unit.state != HexUnit.State.Free)
            {
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
            path.Add(newPoint);

            string previous = path[path.Count - 2];
            GetUnit(StringToVector(previous)).ChangeStatus(HexUnit.State.Cable);
            incompletePaths.Remove(key);
            Vector2 joinPath;
            bool pathCompleted = CheckPathCompleted(cable, out joinPath);
            if (pathCompleted)
            {
                unit.ChangeStatus(HexUnit.State.InUse);
                if (joinPath != Vector2.zero)
                {
                    //It joins a different path
                    string joinPoint = joinPath.x + ":" + joinPath.y;
                    path.Add(joinPoint);
                    Vector2 pathSource = GetUnit(joinPath).source;
                    string pathKey = pathSource.x + ":" + pathSource.y;
                    List<string> p;
                    paths.TryGetValue(pathKey, out p);

                    bool merge = false;

                    for (int i = 0; i < p.Count - 1; i++)
                    {
                        if (!merge)
                        {
                            merge = p[i] == joinPoint;
                        }
                        else
                        {
                            path.Add(p[i]);
                        }
                    }
                }
                CompletePath(path);
            }
            else
            {
                incompletePaths.Add(key, path);
            }

            CheckGridConnections();
        }
    }

    bool CheckPathCompleted(Vector2 coord, out Vector2 joinPath)
    {
        joinPath = Vector2.zero;

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
                    joinPath = current;
                    return true;
                }
            }
        }
        return false;
    }

    void CompletePath(List<string> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            HexUnit unit = GetUnit(StringToVector(path[i]));
            if (unit != null)
            {
                unit.ChangeStatus(HexUnit.State.InUse);
            }
        }
        path.Add("0:0");

        string key = path[0];
        paths.Add(key, path);
        Source s;
        sources.TryGetValue(key, out s);
        if (s != null)
        {
            s.StartExtract();
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

        if (chance > chanceWall + chanceSource)
        {
            chance = Random.value;
            if (chance > oreChance) return HexUnit.Type.Empty;
            return HexUnit.Type.Ore;
        }

        return (chance < chanceSource) ? HexUnit.Type.Source : HexUnit.Type.Wall;
    }

    public void CheckGridConnections()
    {
        foreach (KeyValuePair<string, HexUnit> pair in grid)
        {
            HexUnit unit = pair.Value;
            if (unit.type == HexUnit.Type.Empty)
            {
                unit.ChangeStatus(HexUnit.State.Free);
                Vector2 coord = unit.coords;
                Vector2[] borders = coord.x % 2 == 0 ? evenBorder : oddBorder;

                Vector2 source = Vector2.zero;

                bool possible = false;
                bool end = false;
                for (int i = 0; i < 6 && !end; i++)
                {
                    Vector2 current = coord + borders[i];
                    HexUnit curUnit = GetUnit(current);
                    if (curUnit != null)
                    {
                        if (curUnit.state == HexUnit.State.Cable)
                        {
                            possible = false;
                            end = true;
                        }
                        else if (curUnit.state == HexUnit.State.Trail)
                        {
                            source = curUnit.source;
                            possible = true;
                        }
                    }
                }

                if (possible)
                {
                    unit.ChangeStatus(HexUnit.State.Possible, source);
                }
            }
        }
    }
}
