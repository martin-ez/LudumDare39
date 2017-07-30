using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0f;
    private const float Y_ANGLE_MAX = 75f;

    public float sensivityX = 4f;
    public float sensivityY = 1f;
    public float distance = 5f;
    public float cameraHeight = 2f;

    private Transform _transform;

    private float _currentX = 0f;
    private float _currentY = 0f;


    void Start()
    {
        _transform = GameObject.FindGameObjectWithTag("Char").transform;
    }

    //Method to be called by input controller
    public void CameraRotation(float mouseX, float mouseY)
    {
        _currentX += mouseX * sensivityX;
        _currentY += mouseY * sensivityY;

        _currentY = Mathf.Clamp(_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    void LateUpdate()
    {
        Vector3 _direction = new Vector3(0, 0, -distance);
        Quaternion _rotation = Quaternion.Euler(_currentY, _currentX, 0);
        transform.position = (_transform.position + Vector3.up * cameraHeight) + _rotation * _direction;
        transform.LookAt(_transform.position + Vector3.up * cameraHeight);
    }
}
