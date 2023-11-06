using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed; 
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;

    private Camera camera;
    private CameraInput movement;
    private CameraInput.CameraActions cameraMovement;
    private Vector2 moveVector;
    private Vector2 zoomVector;

    private void Awake()
    {
        movement = new CameraInput();
        cameraMovement = movement.Camera;
        camera = GetComponent<Camera>();
    }

    private void OnMovementPerformed(InputAction.CallbackContext c)
    {
        moveVector = c.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext c)
    {
        moveVector = Vector2.zero;
    }

    private void OnZoomPerformed(InputAction.CallbackContext c)
    {
        zoomVector = c.ReadValue<Vector2>();
    }

    private void OnZoomCancelled(InputAction.CallbackContext c)
    {
        zoomVector = Vector2.zero;
    }
    
    private void Update()
    {
        transform.position += new Vector3(0, 0, zoomVector.y * zoomSpeed);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -750, 700),
            Mathf.Clamp(transform.position.y, -300, 300),
            Mathf.Clamp(transform.position.z, minZoom, maxZoom));
    }

    private void FixedUpdate()
    {
        transform.Translate(moveVector * moveSpeed);
    }

    private void OnEnable()
    {
        movement.Enable();
        cameraMovement.Move.performed += OnMovementPerformed;
        cameraMovement.Move.canceled += OnMovementCancelled;
        cameraMovement.Zoom.performed += OnZoomPerformed;
        cameraMovement.Zoom.canceled += OnZoomCancelled;

    }

    private void OnDisable()
    {
        movement.Disable();
        movement.Camera.Move.performed -= OnMovementPerformed;
        cameraMovement.Move.canceled -= OnMovementCancelled;
        cameraMovement.Zoom.performed -= OnZoomPerformed;
        cameraMovement.Zoom.canceled -= OnZoomCancelled;
    }
}
