using System.Collections;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
    [SerializeField] private Image characterImage;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float durationToDisplay = 5.0f;

    private SSDialogueNodeSO nodeSO;

    public void DisplayDialogue(Sprite characterSprite, string characterName, SSDialogueNodeSO node) {
        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;
        nodeSO = node;
        StartCoroutine(DisplayText(dialogueText, node.Text, 0.01f));
    }

    private IEnumerator DisplayText(TextMeshProUGUI text, string textToDisplay, float speed) {
        int letterIndex = 0;
        string tempText = "";
        string tagBuffer = "";
        bool bufferTag = false;
        text.text = tempText;
        text.enableAutoSizing = false;
        text.fontSize = 20;

        while (letterIndex < textToDisplay.Length) {
            if (text.isTextOverflowing) {
                text.enableAutoSizing = true;
                text.fontSizeMin = 6;
                text.fontSizeMax = 72;
            }

            //If tag-beginning character is parsed, start buffering the tag
            if (textToDisplay[letterIndex] == '<')
                bufferTag = true;
            //If tag-ending character is parsed, buffer the tag ending character and concatenate with text
            if (textToDisplay[letterIndex] == '>') {
                bufferTag = false;
                tagBuffer += textToDisplay[letterIndex];
                tempText = string.Concat(tempText, tagBuffer);
                letterIndex++;
                tagBuffer = ""; //Reset buffer in case of multiple tags.
                continue;
            }

            //If buffering tag, write to buffer instead of tempText and skip waiting
            if (bufferTag) {
                tagBuffer += textToDisplay[letterIndex];
                letterIndex++;
                continue;
            }

            yield return new WaitForSeconds(speed);
            tempText += textToDisplay[letterIndex];
            text.text = tempText;
            letterIndex++;
        }

        yield return new WaitForSeconds(durationToDisplay);

        nodeSO.IsCompleted = true;
    }
}