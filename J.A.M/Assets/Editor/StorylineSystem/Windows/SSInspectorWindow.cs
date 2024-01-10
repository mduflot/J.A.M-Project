using SS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Enumerations;

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

            EnumField enumFieldGraphStatus = SSElementUtility.CreateEnumField(graphView.StoryStatus, "SS Status:",
                callback => { graphView.StoryStatus = (SSStoryStatus)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphStatus);

            EnumField enumFieldGraphType = SSElementUtility.CreateEnumField(graphView.StoryType, "SS Type:",
                callback => { graphView.StoryType = (SSStoryType)callback.newValue; });

            rootVisualElement.Add(enumFieldGraphType);

            Toggle toggleGraph = SSElementUtility.CreateToggle(graphView.IsFirstToPlay, "Is First To Play:",
                callback => { graphView.IsFirstToPlay = callback.newValue; });

            rootVisualElement.Add(toggleGraph);
            
            Toggle toggleGraphReplayable = SSElementUtility.CreateToggle(graphView.IsReplayable, "Is Replayable:",
                callback => { graphView.IsReplayable = callback.newValue; });
            
            rootVisualElement.Add(toggleGraphReplayable);

            ObjectField objectFieldGraphCondition = SSElementUtility.CreateObjectField(graphView.Condition,
                typeof(ConditionSO), "SS Condition:",
                callback => { graphView.Condition = (ConditionSO)callback.newValue; });

            rootVisualElement.Add(objectFieldGraphCondition);

            foreach (var group in graphView.Groups)
            {
                EnumField enumFieldGroupStatus = SSElementUtility.CreateEnumField(group.Value.Groups[0].StoryStatus,
                    $"{group.Value.Groups[0].title} Status:",
                    callback => { group.Value.Groups[0].StoryStatus = (SSStoryStatus)callback.newValue; });

                rootVisualElement.Add(enumFieldGroupStatus);

                Toggle toggle = SSElementUtility.CreateToggle(group.Value.Groups[0].IsFirstToPlay,
                    $"{group.Value.Groups[0].title} Is First To Play:",
                    callback => { group.Value.Groups[0].IsFirstToPlay = callback.newValue; });

                rootVisualElement.Add(toggle);

                UnsignedIntegerField integerFieldMinWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].minWaitTime,
                    $"{group.Value.Groups[0].title} Min Wait Time:",
                    callback => { group.Value.Groups[0].minWaitTime = (uint)callback.newValue; });

                rootVisualElement.Add(integerFieldMinWaitTime);

                UnsignedIntegerField integerFieldMaxWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].maxWaitTime,
                    $"{group.Value.Groups[0].title} Max Wait Time:",
                    callback => { group.Value.Groups[0].maxWaitTime = (uint)callback.newValue; });

                rootVisualElement.Add(integerFieldMaxWaitTime);

                Toggle toggleTimeIsOverride = SSElementUtility.CreateToggle(group.Value.Groups[0].timeIsOverride,
                    $"{group.Value.Groups[0].title} Time Is Override:",
                    callback => { group.Value.Groups[0].timeIsOverride = callback.newValue; });

                rootVisualElement.Add(toggleTimeIsOverride);

                UnsignedIntegerField integerFieldOverrideWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].overrideWaitTime,
                    $"{group.Value.Groups[0].title} Override Wait Time:",
                    callback => { group.Value.Groups[0].overrideWaitTime = (uint)callback.newValue; });

                rootVisualElement.Add(integerFieldOverrideWaitTime);

                ObjectField objectField = SSElementUtility.CreateObjectField(group.Value.Groups[0].Condition,
                    typeof(ConditionSO),
                    $"{group.Value.Groups[0].title} Condition:",
                    callback => { group.Value.Groups[0].Condition = (ConditionSO)callback.newValue; });

                rootVisualElement.Add(objectField);
            }
        }
    }
}