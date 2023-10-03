using nickeltin.Core.Editor;
using nickeltin.SOCreateWindow.Editor;
using UnityEditor;

namespace nickeltin.TextureShapes.Editor
{
    internal static class ContextMenu
    {
        [CustomCreateAssetWindow("nickeltin/TextureShapes", "Ramp", ProducedType = typeof(ShapeContainer))]
        private static void CreateRamp(string atPath)
        {
            CreateShape(new RampShape(), "Ramp", 512, 64);
        }
        
        [CustomCreateAssetWindow("nickeltin/TextureShapes", "Circle", ProducedType = typeof(ShapeContainer))]
        private static void CreateCircle(string atPath)
        {
            CreateShape(new CircleShape(), "Circle", 512, 512);
        }

        private static void CreateShape(TextureShape shape, string name, int width, int height)
        {
            var serializedShape = ShapeSerialization.SerializeToString(shape);
            CreateAssetWithContent.Create($"{name}.{ShapeAssetImporter.EXT}", serializedShape, (o, path) =>
            {
                var importer = (ShapeAssetImporter)AssetImporter.GetAtPath(path);
                importer._majorResolution = width;
                importer._minorResolution = height;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            });
        }
    }
}