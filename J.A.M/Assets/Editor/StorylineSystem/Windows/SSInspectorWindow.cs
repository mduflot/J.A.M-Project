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

            EnumField enumFieldGraphType = ElementUtility.CreateEnumField(graphView.StoryType, "SS Type:",
                callback => { graphView.StoryType = (SSStoryType)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphType);

            ListView conditionsListView = ElementUtility.CreateListViewObjectField(graphView.Conditions, "Conditions:");

            rootVisualElement.Add(conditionsListView);

            foreach (var group in graphView.Groups)
            {
                EnumField enumFieldGroupType = ElementUtility.CreateEnumField(group.Value.Groups[0].StoryType,
                    $"{group.Value.Groups[0].title} Type:",
                    callback => { group.Value.Groups[0].StoryType = (SSStoryType)callback.newValue; });

                rootVisualElement.Add(enumFieldGroupType);

                ListView conditionsListViewGroup = ElementUtility.CreateListViewObjectField(
                    group.Value.Groups[0].Conditions,
                    $"{group.Value.Groups[0].title} Conditions:");

                rootVisualElement.Add(conditionsListViewGroup);
            }
        }
    }
}