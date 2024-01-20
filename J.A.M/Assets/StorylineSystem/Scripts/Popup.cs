using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SS
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI popupTitle;
        [SerializeField] private TextMeshProUGUI popupText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button passTutorialButton;
        
        [HideInInspector] public bool continueButtonPressed;
        [HideInInspector] public bool passTutorialPressed;

        private void OnEnable()
        {
            continueButton.onClick.AddListener(() => gameObject.SetActive(false));
            continueButton.onClick.AddListener(() => continueButtonPressed = true);
            if (passTutorialButton != null)
            {
                passTutorialButton.onClick.AddListener(() => gameObject.SetActive(false));
                passTutorialButton.onClick.AddListener(() => passTutorialPressed = true);
            }
        }
        
        private void OnDisable()
        {
            continueButton.onClick.RemoveAllListeners();
            if (passTutorialButton != null) passTutorialButton.onClick.RemoveAllListeners();
        }

        public void Initialize(string text, string title = null)
        {
            gameObject.SetActive(true);
            if (title != null && popupTitle != null) popupTitle.text = title;
            StartCoroutine(DisplayText(popupText, text, 0.02f));
            continueButtonPressed = false;
            if (passTutorialButton != null) passTutorialPressed = false;
        }

        public void InitializeEndGame(string text, string title)
        {
            gameObject.SetActive(true);
            popupTitle.text = title;
            StartCoroutine(DisplayText(popupText, text, 0.02f));
            continueButton.onClick.AddListener(() => GameManager.Instance.MenuManager.LoadScene("MenuScene"));
        }
        
        private IEnumerator DisplayText(TextMeshProUGUI text, string textToDisplay, float speed)
        {
            int letterIndex = 0;
            string tempText = "";
            string tagBuffer = "";
            bool bufferTag = false;
            text.text = tempText;

            while (letterIndex < textToDisplay.Length)
            {
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
        }
    }
}