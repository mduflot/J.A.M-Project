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
                    userData = SSNodeType.DIALOGUE
                },
                new SearchTreeEntry(new GUIContent("Task", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.TASK
                },
                new SearchTreeEntry(new GUIContent("Time", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.TIME
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
                case SSNodeType.DIALOGUE:
                {
                    SSDialogueNode dialogueNode =
                        (SSDialogueNode)graphView.CreateNode("DialogueNode", SSNodeType.DIALOGUE, localMousePosition);

                    graphView.AddElement(dialogueNode);

                    return true;
                }

                case SSNodeType.TASK:
                {
                    SSTaskNode taskNode =
                        (SSTaskNode)graphView.CreateNode("TaskNode", SSNodeType.TASK, localMousePosition);

                    graphView.AddElement(taskNode);

                    return true;
                }

                case SSNodeType.TIME:
                {
                    SSTimeNode timeNode =
                        (SSTimeNode)graphView.CreateNode("TimeName", SSNodeType.TIME, localMousePosition);

                    graphView.AddElement(timeNode);

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