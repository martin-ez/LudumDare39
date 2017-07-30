using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public GameObject hexUnitPrefab;
    public float chanceSource = 0.1f;
    public float chanceWall = 0.2f;
    public System.Action Pulse;

    Dictionary<string, HexUnit> _grid;
    bool active;
    float factor;
    float unitSize = 50f;

    float pulseInterval = 0.75f;
    float nextPulse;

    void Awake()
    {
        active = true;
        _grid = new Dictionary<string, HexUnit>();
        factor = Mathf.Sqrt(3);
    }

    void Start()
    {
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
        string key = coords.x + ":" + coords.y;
        if (!_grid.ContainsKey(key) && key != "0:0")
        {
            GameObject unit = Instantiate(hexUnitPrefab);
            unit.transform.SetParent(transform);
            unit.transform.localPosition = ToHexCoords(coords);

            HexUnit hexUnit = unit.GetComponent<HexUnit>();
            hexUnit.coords = coords;
            hexUnit.ChangeType(SelectType());
            hexUnit.Rise();
            _grid.Add(key, hexUnit);
        }
    }

    public HexUnit GetUnit(Vector2 coords)
    {
        string key = coords.x + ":" + coords.y;
        HexUnit unit;
        _grid.TryGetValue(key, out unit);
        return unit;
    }

    Vector3 ToHexCoords(Vector2 coords)
    {
        if (coords.x % 2 == 0)
            return new Vector3(coords.x * 1.5f * unitSize, 0f, coords.y * factor * unitSize);
        return new Vector3(coords.x * 1.5f * unitSize, 0f, (coords.y + 0.5f) * factor * unitSize);
    }

    HexUnit.Type SelectType()
    {
        float chance = Random.value;

        if (chance > chanceWall + chanceSource) return HexUnit.Type.Empty;

        return (chance < chanceSource) ? HexUnit.Type.Source : HexUnit.Type.Wall;
    }
}
