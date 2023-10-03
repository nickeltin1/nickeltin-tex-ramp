using UnityEditor;

namespace nickeltin.TextureShapes.Editor
{
    public abstract class ShapeDrawer 
    {
        internal ShapeDrawer()
        {
            
        }

        public abstract string GetShapeDisplayName();
        
        public abstract void OnShapeGUI(SerializedProperty property);
    }
}