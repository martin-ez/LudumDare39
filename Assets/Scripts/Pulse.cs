using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    HexGrid grid;
    Vector2 source;

    Vector2 fromCoord;
    Vector2 toCoord;

    Vector3 from;
    Vector3 to;
    int amount;

    int i_cable;

    float travelTime = 0.75f;
    float timePassed = 0f;

    void Awake()
    {
        grid = FindObjectOfType<HexGrid>();
    }

    void Start()
    {
        grid.Pulse += OnPulse;
    }

    public void Generate(Vector2 pSource, int pAmount)
    {
        i_cable = 2;
        amount = pAmount;
        source = pSource;

        fromCoord = source;
        toCoord = grid.GetNextCable(source, 1);

        from = HexGrid.ToHexCoords(fromCoord) + (Vector3.up * 10);
        to = HexGrid.ToHexCoords(toCoord) + (Vector3.up * 10);

        transform.position = from;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        float percent = timePassed / travelTime;

        transform.position = Vector3.Lerp(from, to, percent);
    }

    void OnPulse()
    {
        transform.position = to;

        if (toCoord == Vector2.zero)
        {
            FindObjectOfType<Base>().GetPower(amount);
            Destroy(gameObject);
        }
        else
        {
            from = to;
            Vector2 temp = toCoord;
            toCoord = grid.GetNextCable(source, i_cable);
            fromCoord = temp;
            to = HexGrid.ToHexCoords(toCoord) + (Vector3.up * 10);
            i_cable++;
            //Lost energy
            amount--;
        }
    }
}
