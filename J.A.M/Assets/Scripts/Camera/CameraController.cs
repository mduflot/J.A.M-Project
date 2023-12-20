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
            origin = GetMousePosition;
        isDragging = ctx.started || ctx.performed;
    }
    

    private void Update()
    {
        transform.position += new Vector3(0, 0, zoomVector.y * zoomSpeed);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -750, 700),
            Mathf.Clamp(transform.position.y, -300, 300),
            Mathf.Clamp(transform.position.z, minZoom, maxZoom));
        if (!isDragging) return;
        difference = GetMousePosition - transform.position;
        transform.position = origin - difference;
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
        cameraMovement.Escape.performed += OnEscapePerformed;
        cameraMovement.Cheat.performed += OnCheatPerformed;
        cameraMovement.Drag.performed += OnDrag;
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
        cameraMovement.Drag.performed -= OnDrag;
    }
    
    //Essaie d'ajouter un offset sur l'axe de profondeur (je suppose que screen to worldpoint prends en compte
    //l'axe Z et du coup déplace la caméra sur le meme plan que le background / vaisseau)
    private Vector3 GetMousePosition => GameManager.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}