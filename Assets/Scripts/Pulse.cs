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

    float travelTime = 1.15f;
    float timePassed = 0f;

    void Awake()
    {
        grid = FindObjectOfType<HexGrid>();
    }

    public void Generate(Vector2 pSource, int pAmount)
    {
        i_cable = 1;
        amount = pAmount;
        source = pSource;

        fromCoord = source;
        toCoord = grid.GetNextCable(source, i_cable);

        from = HexGrid.ToHexCoords(fromCoord);
        to = HexGrid.ToHexCoords(toCoord);

        transform.position = from;
        transform.LookAt(to);
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        float percent = timePassed / travelTime;

        transform.position = Vector3.Lerp(from, to, percent);

        if (percent >= 1)
        {
            NextTarget();
        }
    }

    void NextTarget()
    {
        if (toCoord == Vector2.zero)
        {
            FindObjectOfType<Base>().GetPower(amount);
            Destroy(gameObject);
        }
        else
        {
            i_cable++;
            from = to;
            fromCoord = toCoord;
            toCoord = grid.GetNextCable(source, i_cable);
            to = HexGrid.ToHexCoords(toCoord);
            transform.LookAt(to);

            timePassed = 0;
            //Lost energy
            if (amount > 0)
            {
                amount--;
            }
        }
    }
}
