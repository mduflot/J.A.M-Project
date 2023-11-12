using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CharacterUI baseParentScript;
    private Transform parentAfterDrag;
    private CharacterUI parentScript;
    [SerializeField] private Image image;
    [SerializeField] private Image currentTaskImage;
    public CharacterBehaviour character;
    
    public void Initialize(CharacterBehaviour c, CharacterUI script)
    {
        baseParentScript = script;
        character = c;
        image.sprite = character.GetCharacterData().characterIcon;
        parentScript = script;
    }
    public void ResetTransform()
    {
        transform.SetParent(baseParentScript.transform);
        transform.position = baseParentScript.transform.position;
        baseParentScript.icon = this;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        parentScript.ClearCharacter();
        image.raycastTarget = false;
        transform.SetParent(GameManager.Instance.UIManager.canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        //transform.position = parentAfterDrag.position;
        parentScript.icon = this;
        image.raycastTarget = true;
    }

    public void AssignTask(TaskDataScriptable t)
    {
        currentTaskImage.sprite = t.taskIcon;
        currentTaskImage.enabled = true;
    }

    public void RefreshIcon()
    {
        if (character.IsWorking())
        {
            AssignTask(character.GetTask().taskData);
        }
        else
        {
            StopTask();
        }
    }

    private void StopTask()
    {
        currentTaskImage.enabled = false;
    }

    public void SetupIcon(Transform parent, CharacterUI script)
    {
        parentAfterDrag = parent;
        parentScript = script;
    }
}
