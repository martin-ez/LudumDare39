using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapTrigger : MonoBehaviour
{
    HexUnit unit;

    public enum Border
    {
        X_PosY,
        X_NegY,
        PosX_Y,
        NegX_Y,
        PosX_PosY,
        NegX_PosY
    }
    public Border border;

    public void AssignHexUnit(HexUnit u)
    {
        unit = u;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Char"))
        {
            unit.MapTriggerEnter(border);
            Destroy(gameObject);
        }
    }
}
