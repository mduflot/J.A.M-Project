using SS.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SS.Windows
{
    public class SSInspectorWindow : GraphViewToolWindow
    {
        protected override string ToolName => "SS Inspector";
        private SSGraphView graphView;

        public void Initialize(SSGraphView ssGraphView)
        {
            graphView = ssGraphView;
        }

        protected override void OnGraphViewChanging() { }

        protected override void OnGraphViewChanged() { }

        public void DisplayInspector()
        {
            rootVisualElement.Clear();
            if (graphView == null) return;

            EnumField enumFieldGraphStatus = ElementUtility.CreateEnumField(graphView.StoryStatus, "SS Status:",
                callback => { graphView.StoryStatus = (SSStoryStatus)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphStatus);

            EnumField enumFieldGraphType = ElementUtility.CreateEnumField(graphView.StoryType, "SS Type:",
                callback => { graphView.StoryType = (SSStoryType)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphType);
            
            Toggle toggleGraph = ElementUtility.CreateToggle(graphView.IsFirstToPlay, "Is First To Play:",
                callback => { graphView.IsFirstToPlay = callback.newValue; });
            
            rootVisualElement.Add(toggleGraph);

            ListView conditionsListView = ElementUtility.CreateListViewObjectField(graphView.Conditions, "Conditions:");

            rootVisualElement.Add(conditionsListView);

            foreach (var group in graphView.Groups)
            {
                EnumField enumFieldGroupStatus = ElementUtility.CreateEnumField(group.Value.Groups[0].StoryStatus,
                    $"{group.Value.Groups[0].title} Status:",
                    callback => { group.Value.Groups[0].StoryStatus = (SSStoryStatus)callback.newValue; });

                rootVisualElement.Add(enumFieldGroupStatus);

                Toggle toggle = ElementUtility.CreateToggle(group.Value.Groups[0].IsFirstToPlay,
                    $"{group.Value.Groups[0].title} Is First To Play:",
                    callback => { group.Value.Groups[0].IsFirstToPlay = callback.newValue; });

                rootVisualElement.Add(toggle);

                ListView conditionsListViewGroup = ElementUtility.CreateListViewObjectField(
                    group.Value.Groups[0].Conditions,
                    $"{group.Value.Groups[0].title} Conditions:");

                rootVisualElement.Add(conditionsListViewGroup);
            }
        }
    }
}