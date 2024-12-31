using Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecapUI : MonoBehaviour
{
    [SerializeField] private Image taskIcon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI outcomesText;
    [SerializeField] private Animator animator;

    public void Initialize(Task task)
    {
        taskIcon.sprite = task.Icon;
        titleText.text = task.Name;
        outcomesText.text = task.previewText;
        Appear(true);
    }

    public void Appear(bool state)
    {
        switch (state)
        {
            case true:
                animator.SetBool("Appear", state);
                break;
            case false:
                animator.SetBool("Appear", state);
                break;
        }
    }
}