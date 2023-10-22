using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Utilities;

    public class SSEditorWindow : EditorWindow
    {
        private SSGraphView graphView;
        private readonly string defaultFileName = "FileName";

        private TextField fileNameTextField;
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
            graphView = new SSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = SSElementUtility.CreateTextField(defaultFileName, "File Name:",
                callback =>
                {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });

            saveButton = SSElementUtility.CreateButton("Save", () => Save());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);

            toolbar.AddStyleSheets("StorylineSystem/SSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        #region Toolbar Actions

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in valid.",
                    "Roger!");

                return;
            }

            SSIOUtility.Initialize(graphView, fileNameTextField.value);
            SSIOUtility.Save();
        }

        #endregion

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