using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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
        private Button miniMapButton;

        [MenuItem("Window/StorylineSystem/Storyline Graph")]
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

            fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "File Name:",
                callback =>
                {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });

            saveButton = ElementUtility.CreateButton("Save", () => Save());

            Button loadButton = ElementUtility.CreateButton("Load", () => Load());
            Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = ElementUtility.CreateButton("Reset", () => ResetGraph());
            miniMapButton = ElementUtility.CreateButton("MiniMap", () => ToggleMiniMap());
            Button inspectorButton = ElementUtility.CreateButton("Inspector", () => ToggleInspector());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);
            toolbar.Add(inspectorButton);

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
            string filePath =
                EditorUtility.OpenFilePanel("Node Graphs", "Assets/Editor/StorylineSystem/Graphs", "asset");

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

        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();

            miniMapButton.ToggleInClassList(".ss-toolbar__button__selected");
        }
        
        private void ToggleInspector()
        {
            SSInspectorWindow inspectorWindow = GetWindow<SSInspectorWindow>();
            inspectorWindow.Initialize(graphView);
            inspectorWindow.DisplayInspector();
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