using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UIManager UIManager;
    public SpaceshipManager SpaceshipManager;
    public Camera mainCamera;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        mainCamera = Camera.main;
    }

    public void RefreshCharacterIcons()
    {
        UIManager.RefreshCharacterIcons();
    }
}
