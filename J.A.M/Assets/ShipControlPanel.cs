using UnityEngine;

public class ShipControlPanel : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Appear(bool state)
    {
        animator.SetBool("Appear", state);
    }
}
