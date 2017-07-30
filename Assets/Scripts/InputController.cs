using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Char _charController;
    private CameraController _cameraController;

    private Transform _cam;
    private Vector3 _movement;

    public System.Action Interact;

    void Start()
    {
        _cam = Camera.main.transform;

        _charController = FindObjectOfType<Char>();
        _cameraController = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        _cameraController.CameraRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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