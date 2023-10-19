using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UIManager UIManager;
    public SpaceshipManager SpaceshipManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
