using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private GameObject cheatContainer;

    private Camera camera;
    private CameraInput movement;
    private CameraInput.CameraActions cameraMovement;
    private Vector2 moveVector;
    private Vector2 zoomVector;

    private Vector3 origin;
    private Vector3 difference;
    private bool isDragging;

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

    private void OnEscapePerformed(InputAction.CallbackContext obj)
    {
        if (menuContainer) menuContainer.SetActive(!menuContainer.activeSelf);
    }
    
    private void OnCheatPerformed(InputAction.CallbackContext obj)
    {
        if (cheatContainer) cheatContainer.SetActive(!cheatContainer.activeSelf);
    }

    private void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            origin = GetMousePosition;
        }
        isDragging = ctx.started || ctx.performed;
    }
    

    private void LateUpdate()
    {
        CameraMovement();
        transform.position = ClampCameraToBounds(transform.position);
    }

    private void FixedUpdate()
    {
        transform.Translate(moveVector * moveSpeed);
    }

    private Vector3 ClampCameraToBounds(Vector3 position)
    {
        position = new Vector3(Mathf.Clamp(position.x, -750, 700),
            Mathf.Clamp(position.y, -300, 300),
            Mathf.Clamp(position.z, minZoom, maxZoom));
        return position;
    }

    private void CameraMovement()
    {
        transform.position += new Vector3(0, 0, zoomVector.y * zoomSpeed);
        if (!isDragging) return;
        difference = (Vector3)GetMousePosition - transform.position;
        transform.position = origin - difference;
    }
    
    private void OnEnable()
    {
        movement.Enable();
        cameraMovement.Move.performed += OnMovementPerformed;
        cameraMovement.Move.canceled += OnMovementCancelled;
        cameraMovement.Zoom.performed += OnZoomPerformed;
        cameraMovement.Zoom.canceled += OnZoomCancelled;
        cameraMovement.Escape.performed += OnEscapePerformed;
        cameraMovement.Cheat.performed += OnCheatPerformed;
        cameraMovement.Drag.started += OnDrag;
        cameraMovement.Drag.performed += OnDrag;
        cameraMovement.Drag.canceled += OnDrag;
    }

    private void OnDisable()
    {
        movement.Disable();
        movement.Camera.Move.performed -= OnMovementPerformed;
        cameraMovement.Move.canceled -= OnMovementCancelled;
        cameraMovement.Zoom.performed -= OnZoomPerformed;
        cameraMovement.Zoom.canceled -= OnZoomCancelled;
        cameraMovement.Escape.performed -= OnEscapePerformed;
        cameraMovement.Cheat.performed -= OnCheatPerformed;
        cameraMovement.Drag.started -= OnDrag;
        cameraMovement.Drag.performed -= OnDrag;
        cameraMovement.Drag.canceled -= OnDrag;
    }
    
    private Vector2 GetMousePosition => camera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, -500));
}