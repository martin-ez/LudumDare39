using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreAnimation : MonoBehaviour
{
    private float rotateSpeed = 1f;
    private float radius = 10f;

    private Vector3 _centre;
    private float _angle;

    private void Start()
    {
        radius = Random.Range(20f, 30f);
        rotateSpeed = Random.Range(0.8f, 2.5f);
        _centre = HexGrid.ToHexCoords(GetComponentInParent<HexUnit>().coords) + Vector3.up * Random.Range(5f, 45f);
    }

    private void Update()
    {

        _angle += rotateSpeed * Time.deltaTime;

        var offset = new Vector3(Mathf.Sin(_angle), 0, Mathf.Cos(_angle)) * radius;
        transform.position = _centre + offset;
    }


}
