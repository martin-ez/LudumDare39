using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    public GameObject[] prefabEmpty;
    public GameObject[] prefabWall;
    public GameObject[] prefabSource;
    public GameObject prefabCable;

    HexGrid grid;

    public enum Type
    {
        Empty,
        Wall,
        Source,
        Cable
    }
    public Type type { get; private set; }
    public Vector2 coords;

    void Start()
    {
        grid = FindObjectOfType<HexGrid>();
        MapTrigger[] trigger = GetComponentsInChildren<MapTrigger>();
        for (int i = 0; i < trigger.Length; i++)
        {
            trigger[i].AssignHexUnit(this);
        }
    }

    public void Rise()
    {
        StartCoroutine(RiseCorutine());
    }

    IEnumerator RiseCorutine()
    {
        Vector3 start = transform.position + Vector3.down * 50;
        Vector3 finish = transform.position;
        float length = 0.25f;
        float timePassed = 0f;
        float percent = 0f;
        while (percent < 1)
        {
            transform.position = Vector3.Lerp(start, finish, percent);
            timePassed += Time.deltaTime;
            percent = timePassed / length;
            yield return null;
        }
        transform.position = finish;
    }

    public void ChangeType(Type pType)
    {
        type = pType;
        GameObject objectsInUnit = null;
        switch (type)
        {
            case Type.Empty:
                objectsInUnit = prefabEmpty[Random.Range(0, prefabEmpty.Length)];
                break;
            case Type.Wall:
                objectsInUnit = prefabWall[Random.Range(0, prefabWall.Length)];
                break;
            case Type.Source:
                objectsInUnit = prefabSource[Random.Range(0, prefabSource.Length)];
                break;
            case Type.Cable:
                objectsInUnit = prefabCable;
                break;
        }

        if (objectsInUnit != null)
        {
            GameObject instance = Instantiate(objectsInUnit);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
        }

        if (type == Type.Source)
        {
            FindObjectOfType<HexGrid>().Pulse += SourcePulse;
        }
    }

    void SourcePulse()
    {

    }

    void OnInteract()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Char"))
        {
            FindObjectOfType<InputController>().Interact += OnInteract;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Char"))
        {
            FindObjectOfType<InputController>().Interact -= OnInteract;
        }
    }

    public void MapTriggerEnter(MapTrigger.Border border)
    {
        Vector2 coordsNew = coords;
        switch (border)
        {
            case MapTrigger.Border.X_PosY:
                coordsNew += new Vector2(0, 1);
                break;
            case MapTrigger.Border.X_NegY:
                coordsNew += new Vector2(0, -1);
                break;
            case MapTrigger.Border.PosX_Y:
                coordsNew += new Vector2(1, coords.x % 2 == 0 ? -1 : 0);
                break;
            case MapTrigger.Border.NegX_Y:
                coordsNew += new Vector2(-1, coords.x % 2 == 0 ? -1 : 0);
                break;
            case MapTrigger.Border.PosX_PosY:
                coordsNew += new Vector2(1, coords.x % 2 == 0 ? 0 : 1);
                break;
            case MapTrigger.Border.NegX_PosY:
                coordsNew += new Vector2(-1, coords.x % 2 == 0 ? 0 : 1);
                break;
        }

        grid.CreateUnit(coordsNew);
    }
}
