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
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = SSNodeType.MultipleChoice
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
                        (SSSingleChoiceNode)graphView.CreateNode(SSNodeType.SingleChoice, localMousePosition);

                    graphView.AddElement(singleChoiceNode);

                    return true;
                }
                
                case SSNodeType.MultipleChoice:
                {
                    SSMultipleChoiceNode multipleChoiceNode =
                        (SSMultipleChoiceNode)graphView.CreateNode(SSNodeType.MultipleChoice, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case Group _:
                {
                    Group group = (Group) graphView.CreateGroup("NodeGroup", localMousePosition);

                    graphView.AddElement(group);
                    
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