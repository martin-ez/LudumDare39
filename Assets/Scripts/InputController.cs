using UnityEngine;

public class InputController : MonoBehaviour
{
    private Character _charController;
    private Camera camRef;

    private Transform _cam;
    private Vector3 _movement;

    private float zoom = 80f;
    private float zoomMin = 20f;
    private float zoomMax = 250f;
    private float zoomSpeed = 7.5f;
    private float zoomSensitivity = 75f;

    public System.Action Interact;

    void Start()
    {
        camRef = Camera.main;
        camRef.orthographicSize = zoom;
        _cam = camRef.transform;

        _charController = FindObjectOfType<Character>();
    }

    void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
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

    void LateUpdate()
    {
        camRef.orthographicSize = Mathf.Lerp(camRef.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
    }
}