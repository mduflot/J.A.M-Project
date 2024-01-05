using UnityEngine;

public class HoverMenu : MonoBehaviour
{
    public virtual void Initialize(HoverMenuData data)
    {
        gameObject.SetActive(true);
    }

    public void QuitMenu(HoverMenuData data)
    {
        transform.parent = data.baseParent;
        gameObject.SetActive(false);
    }
}

public class HoverMenuData
{
    public string text1;
    public Transform baseParent;
    public Transform parent;
    public string text2;
}
