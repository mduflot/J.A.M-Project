using SS.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Windows
{
    public class SSInspectorWindow : GraphViewToolWindow
    {
        protected override string ToolName { get; } = "SS Inspector";
        private SSGraphView graphView;

        public void Initialize(SSGraphView ssGraphView)
        {
            graphView = ssGraphView;
        }

        protected override void OnGraphViewChanging()
        {
        }

        protected override void OnGraphViewChanged()
        {
        }

        public void DisplayInspector()
        {
            rootVisualElement.Clear();
            if (graphView == null) return;

            EnumField enumFieldGraphStatus = ElementUtility.CreateEnumField(graphView.storyStatus, "SS Status:",
                callback => { graphView.storyStatus = (SSStoryStatus)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphStatus);

            EnumField enumFieldGraphType = ElementUtility.CreateEnumField(graphView.storyType, "SS Type:",
                callback => { graphView.storyType = (SSStoryType)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphType);

            foreach (var group in graphView.Groups)
            {
                EnumField enumFieldGroupStatus = ElementUtility.CreateEnumField(group.Value.Groups[0].StoryStatus,
                    $"{group.Value.Groups[0].title} Status:",
                    callback => { group.Value.Groups[0].StoryStatus = (SSStoryStatus)callback.newValue; });

                rootVisualElement.Add(enumFieldGroupStatus);


                EnumField enumFieldGroupType = ElementUtility.CreateEnumField(group.Value.Groups[0].StoryType,
                    $"{group.Value.Groups[0].title} Type:",
                    callback => { group.Value.Groups[0].StoryType = (SSStoryType)callback.newValue; });

                rootVisualElement.Add(enumFieldGroupType);
            }
        }
    }
}