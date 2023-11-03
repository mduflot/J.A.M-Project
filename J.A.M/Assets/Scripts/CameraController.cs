using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;

    private PlayerInputs movement;
    private PlayerInputs.CameraActions cameraMovement;
    
    private Vector2 horizontalInput;
    private Vector2 verticalInput;

    private void Awake()
    {
        movement = new PlayerInputs();
        cameraMovement = movement.Camera;

        cameraMovement.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

    }

    private void Move()
    {
        
    }
}
