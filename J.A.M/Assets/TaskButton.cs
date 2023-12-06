using SS;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SSLauncher))]
public class TaskButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskName;
    [SerializeField] private TextMeshProUGUI taskDuration;
    [SerializeField] private Image taskIcon;
    private SSTaskNodeSO task;
    private SSLauncher launcher;

    private void Start()
    {
        launcher = GetComponent<SSLauncher>();
        task = (SSTaskNodeSO)launcher.node;
        taskName.text = task.name;
        taskDuration.text = task.Duration + " hours";
        taskIcon.sprite = task.Icon;
    }
}
