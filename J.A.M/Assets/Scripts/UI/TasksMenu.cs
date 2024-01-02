using UnityEngine;
using UnityEngine.EventSystems;

public class TasksMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject permanentTasksMenu;
    [SerializeField] private GameObject emergencyTasksMenu;
    [SerializeField] private Animator permanentButton;
    [SerializeField] private Animator emergencyButton;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ResetButtons();
    }

    public void ClickedMenu(bool emergency)
    {
        animator.SetBool("Opened", true);
        DisplayTasks(emergency);
    }

    private void DisplayTasks(bool emergency)
    {
        emergencyButton.SetBool("Selected", emergency);
        permanentButton.SetBool("Selected", !emergency);
        
        emergencyTasksMenu.SetActive(emergency);
        permanentTasksMenu.SetActive(!emergency);
    }

    private void CloseMenu()
    {
        animator.SetBool("Opened", false);
        ResetButtons();
    }

    private void ResetButtons()
    {
        permanentButton.SetBool("Selected", true);
        emergencyButton.SetBool("Selected", false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseMenu();
    }
}