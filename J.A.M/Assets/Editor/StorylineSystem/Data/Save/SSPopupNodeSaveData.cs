using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;

    [Serializable]
    public class SSPopupNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public SSPopupUIType PopupUIType { get; set; }
    }
}