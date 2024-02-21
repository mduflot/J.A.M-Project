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
            
            Foldout textFoldout = SSElementUtility.CreateFoldout("Storyline :");

            EnumField enumFieldGraphStatus = SSElementUtility.CreateEnumField(graphView.StoryStatus, "SS Status:",
                callback => { graphView.StoryStatus = (SSStoryStatus)callback.newValue; });

            textFoldout.Add(enumFieldGraphStatus);

            EnumField enumFieldGraphType = SSElementUtility.CreateEnumField(graphView.StoryType, "SS Type:",
                callback => { graphView.StoryType = (SSStoryType)callback.newValue; });

            textFoldout.Add(enumFieldGraphType);
            
            Toggle toggleGraphTutorial = SSElementUtility.CreateToggle(graphView.IsTutorialToPlay, "Is Tutorial To Play:",
                callback => { graphView.IsTutorialToPlay = callback.newValue; });
            
            textFoldout.Add(toggleGraphTutorial);

            Toggle toggleGraph = SSElementUtility.CreateToggle(graphView.IsFirstToPlay, "Is First To Play:",
                callback => { graphView.IsFirstToPlay = callback.newValue; });

            textFoldout.Add(toggleGraph);
            
            Toggle toggleGraphReplayable = SSElementUtility.CreateToggle(graphView.IsReplayable, "Is Replayable:",
                callback => { graphView.IsReplayable = callback.newValue; });
            
            textFoldout.Add(toggleGraphReplayable);

            ObjectField objectFieldGraphCondition = SSElementUtility.CreateObjectField(graphView.Condition,
                typeof(ConditionSO), "SS Condition:",
                callback => { graphView.Condition = (ConditionSO)callback.newValue; });

            textFoldout.Add(objectFieldGraphCondition);
            
            rootVisualElement.Add(textFoldout);

            foreach (var group in graphView.Groups)
            {
                Foldout textFoldoutGroup = SSElementUtility.CreateFoldout($"Timeline : {group.Value.Groups[0].title}");
                
                EnumField enumFieldGroupStatus = SSElementUtility.CreateEnumField(group.Value.Groups[0].StoryStatus,
                    $"Status:",
                    callback => { group.Value.Groups[0].StoryStatus = (SSStoryStatus)callback.newValue; });
                textFoldoutGroup.Add(enumFieldGroupStatus);

                Toggle toggle = SSElementUtility.CreateToggle(group.Value.Groups[0].IsFirstToPlay,
                    $"Is First To Play:",
                    callback => { group.Value.Groups[0].IsFirstToPlay = callback.newValue; });
                textFoldoutGroup.Add(toggle);

                UnsignedIntegerField integerFieldMinWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].minWaitTime,
                    $"Min Wait Time:",
                    callback => { group.Value.Groups[0].minWaitTime = (uint)callback.newValue; });
                textFoldoutGroup.Add(integerFieldMinWaitTime);

                UnsignedIntegerField integerFieldMaxWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].maxWaitTime,
                    $"Max Wait Time:",
                    callback => { group.Value.Groups[0].maxWaitTime = (uint)callback.newValue; });
                textFoldoutGroup.Add(integerFieldMaxWaitTime);

                Toggle toggleTimeIsOverride = SSElementUtility.CreateToggle(group.Value.Groups[0].timeIsOverride,
                    $"Time Is Override:",
                    callback => { group.Value.Groups[0].timeIsOverride = callback.newValue; });
                textFoldoutGroup.Add(toggleTimeIsOverride);

                UnsignedIntegerField integerFieldOverrideWaitTime = SSElementUtility.CreateUnsignedIntegerField(group.Value.Groups[0].overrideWaitTime,
                    $"Override Wait Time:",
                    callback => { group.Value.Groups[0].overrideWaitTime = (uint)callback.newValue; });
                textFoldoutGroup.Add(integerFieldOverrideWaitTime);

                ObjectField objectField = SSElementUtility.CreateObjectField(group.Value.Groups[0].Condition,
                    typeof(ConditionSO),
                    $"Condition:",
                    callback => { group.Value.Groups[0].Condition = (ConditionSO)callback.newValue; });
                textFoldoutGroup.Add(objectField);

                rootVisualElement.Add(textFoldoutGroup);
            }
        }
    }
}