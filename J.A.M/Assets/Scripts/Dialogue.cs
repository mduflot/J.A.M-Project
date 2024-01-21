using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float durationToDisplay = 15.0f;
    [SerializeField] private float durationBeforeDestroy = 45.0f;

    private float duration;
    private float durationDestroy;
    private SSDialogueNodeSO nodeSO;
    private DialogueManager dialogueManager;

    public void DisplayDialogue(Sprite characterSprite, string characterName, SSDialogueNodeSO node, DialogueManager dialogueManager)
    {
        durationDestroy = durationBeforeDestroy * TimeTickSystem.ticksPerHour;
        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;
        nodeSO = node;
        this.dialogueManager = dialogueManager;
        duration = durationToDisplay * TimeTickSystem.ticksPerHour;
        StartCoroutine(DisplayText(dialogueText, node.Text, 0.01f));
    }

    private void DisplayDialogue(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (duration > 0)
        {
            duration -= TimeTickSystem.ticksPerHour;
            return;
        }

        nodeSO.IsCompleted = true;
        TimeTickSystem.OnTick -= DisplayDialogue;
        TimeTickSystem.OnTick += DestroyDialogue;
    }

    private void DestroyDialogue(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (durationDestroy > 0)
        {
            durationDestroy -= TimeTickSystem.ticksPerHour;
            return;
        }

        TimeTickSystem.OnTick -= DestroyDialogue;
        if (!dialogueManager.dialogueQueue.Contains(this)) return;
        dialogueManager.dialogueQueue = new Queue<Dialogue>(dialogueManager.dialogueQueue.Where(dialogue => dialogue != this));
        StartCoroutine(Fade());
    }

    private IEnumerator DisplayText(TextMeshProUGUI text, string textToDisplay, float speed)
    {
        int letterIndex = 0;
        string tempText = "";
        string tagBuffer = "";
        bool bufferTag = false;
        text.text = tempText;
        text.enableAutoSizing = false;
        text.fontSize = 20;

        while (letterIndex < textToDisplay.Length)
        {
            if (text.isTextOverflowing)
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = 6;
                text.fontSizeMax = 72;
            }

            //If tag-beginning character is parsed, start buffering the tag
            if (textToDisplay[letterIndex] == '<')
                bufferTag = true;
            //If tag-ending character is parsed, buffer the tag ending character and concatenate with text
            if (textToDisplay[letterIndex] == '>')
            {
                bufferTag = false;
                tagBuffer += textToDisplay[letterIndex];
                tempText = string.Concat(tempText, tagBuffer);
                letterIndex++;
                tagBuffer = ""; //Reset buffer in case of multiple tags.
                continue;
            }

            //If buffering tag, write to buffer instead of tempText and skip waiting
            if (bufferTag)
            {
                tagBuffer += textToDisplay[letterIndex];
                letterIndex++;
                continue;
            }

            yield return new WaitForSeconds(speed);
            tempText += textToDisplay[letterIndex];
            text.text = tempText;
            letterIndex++;
        }

        TimeTickSystem.OnTick += DisplayDialogue;
    }

    public IEnumerator Fade()
    {
        while (characterImage.color.a > 0)
        {
            characterImage.color = new Color(characterImage.color.r, characterImage.color.g, characterImage.color.b,
                characterImage.color.a - 0.01f);
            characterNameText.color = new Color(characterNameText.color.r, characterNameText.color.g,
                characterNameText.color.b, characterNameText.color.a - 0.1f);
            dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b,
                dialogueText.color.a - 0.1f);
            background.color = new Color(background.color.r, background.color.g, background.color.b,
                background.color.a - 0.1f);
            yield return new WaitForSeconds(0.01f);
        }

        if (dialogueManager.dialogueQueue.Contains(this))
            dialogueManager.dialogueQueue = new Queue<Dialogue>(dialogueManager.dialogueQueue.Where(dialogue => dialogue != this));
        Destroy(gameObject);
    }
}