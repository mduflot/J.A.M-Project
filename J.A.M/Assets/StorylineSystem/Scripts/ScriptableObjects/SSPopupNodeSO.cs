using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class SSPopupNodeSO : SSNodeSO
    {
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public SSPopupUIType PopupUIType { get; set; }
        [field: SerializeField] public bool IsTutorialPopup { get; set; }

        public void Initialize(string nodeName, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, SSPopupUIType popupUiType, bool isTutorialPopup)
        {
            NodeName = nodeName;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            PopupUIType = popupUiType;
            IsTutorialPopup = isTutorialPopup;
        }
    }
}