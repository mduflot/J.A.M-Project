using System;
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
        public uint minWaitTime { get; set; }
        public uint maxWaitTime { get; set; }
        public bool timeIsOverride { get; set; }
        public uint overrideWaitTime { get; set; }
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
            minWaitTime = 5;
            maxWaitTime = 10;
            timeIsOverride = false;
            overrideWaitTime = 10;
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