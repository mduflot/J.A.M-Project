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
                new SearchTreeEntry(new GUIContent("Event Multiple", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Event
                },
                new SearchTreeEntry(new GUIContent("Start", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Start
                },
                new SearchTreeEntry(new GUIContent("End", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.End
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
                        (SSDialogueNode)graphView.CreateNode("NodeName", SSNodeType.Dialogue, localMousePosition);

                    graphView.AddElement(dialogueNode);

                    return true;
                }
                
                case SSNodeType.Event:
                {
                    SSEventNode eventNode =
                        (SSEventNode)graphView.CreateNode("NodeName", SSNodeType.Event, localMousePosition);

                    graphView.AddElement(eventNode);

                    return true;
                }

                case SSNodeType.Start:
                {
                    SSStartNode startNode =
                        (SSStartNode)graphView.CreateNode("NodeName", SSNodeType.Start, localMousePosition);
                    
                    graphView.AddElement(startNode);
                    
                    return true;
                }

                case SSNodeType.End:
                {
                    SSEndNode endNode =
                        (SSEndNode)graphView.CreateNode("NodeName", SSNodeType.End, localMousePosition);
                    
                    graphView.AddElement(endNode);
                    
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