using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Elements
{
    using Enumerations;

    public class SSGroup : Group
    {
        public string ID { get; set; }
        public SSStoryStatus StoryStatus { get; set; }
        public bool IsFirstToPlay { get; set; }
        public ConditionSO Condition { get; set; }
        public string OldTitle { get; set; }

        private Color defaultBorderColor;
        private float defaultBorderWidth;

        public SSGroup(string groupTitle, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();

            title = groupTitle;
            StoryStatus = SSStoryStatus.Enabled;
            IsFirstToPlay = false;
            OldTitle = groupTitle;

            SetPosition(new Rect(position, Vector2.zero));

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
    }
}