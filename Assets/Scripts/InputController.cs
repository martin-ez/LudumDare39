using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Character _charController;
    private Camera camRef;

    private Transform _cam;
    private Vector3 _movement;

    private float camSmooth = 5f;
    private float camSizeMin = 20f;
    private float camSizeMax = 250f;

    public System.Action Interact;

    void Start()
    {
        camRef = Camera.main;
        _cam = camRef.transform;

        _charController = FindObjectOfType<Character>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0)
        {
            camRef.orthographicSize = Mathf.Clamp(camRef.orthographicSize + camSmooth, camSizeMin, camSizeMax);
        }
        else if (scroll > 0)
        {
            camRef.orthographicSize = Mathf.Clamp(camRef.orthographicSize - camSmooth, camSizeMin, camSizeMax);
        }
    }

    void FixedUpdate()
    {
        bool _interact = Input.GetAxis("Interact") > 0;
        float _h = Input.GetAxis("Horizontal");
        float _v = Input.GetAxis("Vertical");

        Vector3 _camForward = Vector3.Scale(_cam.forward, new Vector3(1, 0, 1)).normalized;
        _movement = _v * _camForward + _h * _cam.right;

        _charController.Move(_movement);

        if (_interact && Interact != null)
        {
            Interact();
        }
    }
}