using nickeltin.SOCreateWindow.Editor;
using UnityEditor;

namespace nickeltin.TextureShapes.Editor
{
    internal static class ContextMenu
    {
        [CustomCreateAssetWindow("nickeltin/TextureShapes", "Ramp", ProducedType = typeof(ShapeContainer))]
        private static void CreateRamp(string atPath)
        {
            CreateShape(new RampShape(), "Ramp");
        }

        private static void CreateShape(TextureShape shape, string name)
        {
            var serializedShape = ShapeSerialization.SerializeToString(shape);
            ProjectWindowUtil.CreateAssetWithContent($"{name}.{ShapeAssetImporter.EXT}", serializedShape);
        }
    }
}