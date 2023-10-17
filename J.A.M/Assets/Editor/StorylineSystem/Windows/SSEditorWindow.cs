using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace SS.Windows
{
    public class SSEditorWindow : EditorWindow
    {
        [MenuItem("Window/SS/Storyline Graph")]
        public static void ShowGraph()
        {
            GetWindow<SSEditorWindow>("Storyline Graph");
        }

        private void CreateGUI()
        {
            AddGraphView();

            AddStyles();
        }

        private void AddGraphView()
        {
            SSGraphView graphView = new SSGraphView();
            
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("StorylineSystem/SSVariables.uss");
            
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}