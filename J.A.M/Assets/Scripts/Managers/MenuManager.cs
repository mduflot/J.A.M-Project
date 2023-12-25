using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsContainer;

    public void SaveGame()
    {
        DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        DataPersistenceManager.Instance.LoadGame();
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenOptions()
    {
        optionsContainer.SetActive(true);
    }

    public void QuitOptions()
    {
        optionsContainer.SetActive(false);
    }

    public void QuitApplication()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}