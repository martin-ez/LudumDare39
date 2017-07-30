using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public int MaxPower = 100;
    int power = 0;

    public void GetPower(int amount)
    {
        power += amount;
    }
}
