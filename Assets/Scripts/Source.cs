using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : MonoBehaviour
{
    public GameObject pulsePrefab;
    public GameObject sourceDestroyedPrefab;

    public int startingPower;
    public int uses;

    int powerLeft;
    int powerRate;

    Vector2 coords;

    void Start()
    {
        powerLeft = startingPower;
        powerRate = startingPower / uses;
        coords = GetComponentInParent<HexUnit>().coords;
    }

    public void StartExtract()
    {
        FindObjectOfType<HexGrid>().Pulse += OnPulse;
    }

    void OnPulse()
    {
        GameObject newPulse = Instantiate(pulsePrefab);

        newPulse.GetComponent<Pulse>().Generate(coords, powerRate);

        powerLeft -= powerRate;
        if (powerLeft <= 0)
        {
            DestroySource();
        }
    }

    void DestroySource()
    {
        FindObjectOfType<HexGrid>().Pulse -= OnPulse;
        GameObject sourceDestroyed = Instantiate(sourceDestroyedPrefab);
        sourceDestroyed.transform.SetParent(transform.parent);
        Destroy(gameObject);
    }
}
