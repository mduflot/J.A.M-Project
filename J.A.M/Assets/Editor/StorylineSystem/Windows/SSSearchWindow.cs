using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Windows
{
    using Elements;
    using Enumerations;

    public class SSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private SSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(SSGraphView ssGraphView)
        {
            graphView = ssGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("CreateElement")),
                new SearchTreeGroupEntry(new GUIContent("Node"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Dialogue
                },
                new SearchTreeEntry(new GUIContent("Task", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Task
                },
                new SearchTreeEntry(new GUIContent("Time", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Time
                },
                new SearchTreeEntry(new GUIContent("Popup", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Popup
                },
                new SearchTreeEntry(new GUIContent("Condition", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.CheckCondition
                },
                new SearchTreeGroupEntry(new GUIContent("Node Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData)
            {
                case SSNodeType.Dialogue:
                {
                    SSDialogueNode dialogueNode =
                        (SSDialogueNode)graphView.CreateNode("DialogueNode", SSNodeType.Dialogue, localMousePosition);

                    graphView.AddElement(dialogueNode);

                    return true;
                }

                case SSNodeType.Task:
                {
                    SSTaskNode taskNode =
                        (SSTaskNode)graphView.CreateNode("TaskNode", SSNodeType.Task, localMousePosition);

                    graphView.AddElement(taskNode);

                    return true;
                }

                case SSNodeType.Time:
                {
                    SSTimeNode timeNode =
                        (SSTimeNode)graphView.CreateNode("TimeName", SSNodeType.Time, localMousePosition);

                    graphView.AddElement(timeNode);

                    return true;
                }
                
                case SSNodeType.Popup:
                {
                    SSPopupNode popupNode =
                        (SSPopupNode)graphView.CreateNode("PopupNode", SSNodeType.Popup, localMousePosition);

                    graphView.AddElement(popupNode);

                    return true;
                }

                case SSNodeType.CheckCondition:
                {
                    SSCheckConditionNode checkConditionNode =
                        (SSCheckConditionNode)graphView.CreateNode("CheckConditionNode", SSNodeType.CheckCondition,
                            localMousePosition);

                    graphView.AddElement(checkConditionNode);

                    return true;
                }

                case Group _:
                {
                    graphView.CreateGroup("NodeGroup", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}