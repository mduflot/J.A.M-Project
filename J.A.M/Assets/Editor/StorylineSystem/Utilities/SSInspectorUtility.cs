using System;
using UnityEditor;
using UnityEngine;

namespace SS.Utilities
{
    public static class SSInspectorUtility
    {
        public static void DrawDisabledFields(Action action)
        {
            EditorGUI.BeginDisabledGroup(true);

            action.Invoke();

            EditorGUI.EndDisabledGroup();
        }

        public static void DrawHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        public static void DrawHelpBox(string message, MessageType messageType = MessageType.Info, bool wide = true)
        {
            EditorGUILayout.HelpBox(message, messageType, wide);
        }

        public static void DrawPropertyField(this SerializedProperty serializedProperty, bool includeChildren = false,
            string label = null)
        {
            if (label == null)
            {
                EditorGUILayout.PropertyField(serializedProperty, includeChildren);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label), includeChildren);
            }
        }

        public static int DrawPopup(string label, SerializedProperty selectedIndexProperty, string[] options)
        {
            return EditorGUILayout.Popup(label, selectedIndexProperty.intValue, options);
        }

        public static int DrawPopup(string label, int selectedIndex, string[] options)
        {
            return EditorGUILayout.Popup(label, selectedIndex, options);
        }

        public static void DrawSpace(int amount = 4)
        {
            EditorGUILayout.Space(amount);
        }
    }
}