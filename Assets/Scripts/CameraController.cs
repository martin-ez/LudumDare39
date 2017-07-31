using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform character;
    private Vector3 offset;    

    void Start()
    {
        character = FindObjectOfType<Character>().transform;
        offset = transform.position - character.position;
    }

    void LateUpdate()
    {       
        transform.position = character.position + offset;
    }
}
