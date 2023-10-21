using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Utilities;
    
    public class SSEditorWindow : EditorWindow
    {
        private readonly string defaultFileName = "NodesFileName";
        private Button saveButton;
        
        [MenuItem("Window/SS/Storyline Graph")]
        public static void ShowGraph()
        {
            GetWindow<SSEditorWindow>("Storyline Graph");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        #region Elements Addition

        private void AddGraphView()
        {
            SSGraphView graphView = new SSGraphView(this);
            
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }
        
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            TextField fileNameTextField = SSElementUtility.CreateTextField(defaultFileName, "File Name:");

            saveButton = SSElementUtility.CreateButton("Save");
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);

            toolbar.AddStyleSheets("StorylineSystem/SSToolbarStyles.uss");
            
            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("StorylineSystem/SSVariables.uss");
        }
        
        #endregion

        #region Utility Methods
        
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        
        #endregion
    }
}