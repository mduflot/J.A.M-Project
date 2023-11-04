using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraInputs : MonoBehaviour
{
    [SerializeField] private float moveSpeed; 
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;

    private CameraInput movement;
    private CameraInput.CameraActions cameraMovement;
    private Vector2 moveVector;

    private void Awake()
    {
        movement = new CameraInput();
        cameraMovement = movement.Camera;
    }

    private void OnMovementPerformed(InputAction.CallbackContext c)
    {
        moveVector = c.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext c)
    {
        moveVector = Vector2.zero;
    }

    private void FixedUpdate()
    {
        transform.Translate(moveVector * moveSpeed);
    }

    private void OnEnable()
    {
        movement.Enable();
        movement.Camera.Move.performed += OnMovementPerformed;
        cameraMovement.Move.canceled += OnMovementCancelled;
    }

    private void OnDisable()
    {
        movement.Disable();
        movement.Camera.Move.performed -= OnMovementPerformed;
        cameraMovement.Move.canceled -= OnMovementCancelled;
    }
}
