using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SS.Windows
{
    public class SSGraphView : GraphView
    {
        public SSGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            
            gridBackground.StretchToParentSize();
            
            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("StorylineSystem/SSGraphViewStyles.uss");
            
            styleSheets.Add(styleSheet);
        }
    }
}