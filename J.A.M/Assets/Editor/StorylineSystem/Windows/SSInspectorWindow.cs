using SS.Enumerations;
using UnityEditor.Experimental.GraphView;
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

        private void Show()
        {
            rootVisualElement.Clear();
            if (graphView == null) return;

            EnumField enumField = ElementUtility.CreateEnumField(graphView.storyStatus, "SS Status:",
                callback => { graphView.storyStatus = (SSStoryStatus)callback.newValue; });

            rootVisualElement.Add(enumField);

            foreach (var group in graphView.Groups)
            {
                EnumField enumFieldGroup = ElementUtility.CreateEnumField(group.Value.Groups[0].StoryStatus, $"{group.Value.Groups[0].title} Status:",
                    callback => { group.Value.Groups[0].StoryStatus = (SSStoryStatus)callback.newValue; });

                rootVisualElement.Add(enumFieldGroup);
            }
        }
    }
}