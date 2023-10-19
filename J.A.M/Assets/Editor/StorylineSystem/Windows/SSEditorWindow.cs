using SS.Utilities;
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
        
        #region Elements Addition

        private void AddGraphView()
        {
            SSGraphView graphView = new SSGraphView();
            
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("StorylineSystem/SSVariables.uss");
        }
        
        #endregion
    }
}