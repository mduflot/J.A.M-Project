using Tasks;
using UnityEngine;

public class HoverMenu : MonoBehaviour
{
    public virtual void Initialize(HoverMenuData data)
    {
        gameObject.SetActive(true);
    }

    public void QuitMenu(HoverMenuData data)
    {
        transform.SetParent(data.baseParent);
        gameObject.SetActive(false);
    }

    public virtual void UpdateMenu(HoverMenuData data)
    {
    }
}

public class HoverMenuData
{
    public string text1;
    public Transform baseParent;
    public Transform parent;
    public string text2;
    public Task Task;
}