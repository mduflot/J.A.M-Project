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
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Event Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.EventMultipleChoice
                },
                new SearchTreeEntry(new GUIContent("Start Choice", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.Start
                },
                new SearchTreeEntry(new GUIContent("End Choice", indentationIcon))
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
                case SSNodeType.SingleChoice:
                {
                    SSSingleChoiceNode singleChoiceNode =
                        (SSSingleChoiceNode)graphView.CreateNode("NodeName", SSNodeType.SingleChoice, localMousePosition);

                    graphView.AddElement(singleChoiceNode);

                    return true;
                }
                
                case SSNodeType.EventMultipleChoice:
                {
                    SSEventMultipleChoiceNode eventMultipleChoiceNode =
                        (SSEventMultipleChoiceNode)graphView.CreateNode("NodeName", SSNodeType.EventMultipleChoice, localMousePosition);

                    graphView.AddElement(eventMultipleChoiceNode);

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