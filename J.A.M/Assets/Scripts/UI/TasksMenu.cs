using UnityEngine;

public class TasksMenu : MonoBehaviour
{
    [SerializeField] private GameObject permanentTasksMenu;
    [SerializeField] private GameObject emergencyTasksMenu;
    [SerializeField] private Animator permanentButton;
    [SerializeField] private Animator emergencyButton;
    private Animator animator;
    private bool isOpen;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ResetButtons();
    }

    public void ClickedMenu(bool emergency)
    {
        if (!isOpen)
        {
            animator.SetBool("Opened", true);
            isOpen = true;
        }   
        else if (emergencyTasksMenu.activeSelf == emergency)
        {
            animator.SetBool("Opened", false);
            ResetButtons();
            isOpen = false;
        }
        DisplayTasks(emergency);
    }

    private void DisplayTasks(bool emergency)
    {
        if (isOpen)
        {
            emergencyButton.SetBool("Selected", emergency);
            permanentButton.SetBool("Selected", !emergency);
        }
        emergencyTasksMenu.SetActive(emergency);
        permanentTasksMenu.SetActive(!emergency);
    }

    private void ResetButtons()
    {
        permanentButton.SetBool("Selected", true);
        emergencyButton.SetBool("Selected", false);
    }
    
}