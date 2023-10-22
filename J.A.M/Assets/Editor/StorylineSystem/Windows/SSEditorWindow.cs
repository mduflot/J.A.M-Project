using System.IO;
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

        private static TextField fileNameTextField;
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

            Button loadButton = SSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = SSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = SSElementUtility.CreateButton("Reset", () => ResetGraph());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);

            toolbar.AddStyleSheets("StorylineSystem/SSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("StorylineSystem/SSVariables.uss");
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
        
        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Node Graphs", "Assets/Editor/StorylineSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            
            Clear();
            
            SSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            SSIOUtility.Load();
        }
        
        private void Clear()
        {
            graphView.ClearGraph();
        }
        
        private void ResetGraph()
        {
            Clear();
            
            UpdateFileName(defaultFileName);
        }

        #endregion

        #endregion

        #region Utility Methods

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        
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