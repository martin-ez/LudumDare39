using UnityEngine;

public class InputController : MonoBehaviour
{
    private Character _charController;
    private Camera camRef;

    private Transform _cam;
    private Vector3 _movement;

    private float zoom = 30f;
    private float zoomMin = 20f;
    private float zoomMax = 104f;
    private float zoomSpeed = 6f;
    private float zoomSensitivity = 40f;

    private float interactCooldown = 0.5f;
    private float nextInteract;


    public System.Action Interact;

    void Start()
    {
        camRef = Camera.main;
        camRef.orthographicSize = zoom;
        camRef.fieldOfView = zoom;
        _cam = camRef.transform;

        _charController = FindObjectOfType<Character>();
        nextInteract = Time.time;
    }

    void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    }

    void FixedUpdate()
    {
        bool _interact = Input.GetAxis("Exit") > 0;
        bool _exit = Input.GetAxis("Interact") > 0;
        float _h = Input.GetAxis("Horizontal");
        float _v = Input.GetAxis("Vertical");
        if (_exit)
        {
            Application.Quit();
        }

        Vector3 _camForward = Vector3.Scale(_cam.forward, new Vector3(1, 0, 1)).normalized;
        _movement = _v * _camForward + _h * _cam.right;

        _charController.Move(_movement);

        if (_interact && Interact != null && Time.time > nextInteract)
        {
            nextInteract = Time.time + interactCooldown;
            Interact();
        }
    }

    void LateUpdate()
    {
        camRef.orthographicSize = Mathf.Lerp(camRef.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
        camRef.fieldOfView = Mathf.Lerp(camRef.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
    }
}