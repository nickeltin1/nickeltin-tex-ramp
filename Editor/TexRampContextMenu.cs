using nickeltin.SOCreateWindow.Editor;
using UnityEditor;

namespace nickeltin.TexRamp.Editor
{
    internal static class TexRampContextMenu
    {
        [CustomCreateAssetWindow("nickeltin/TexRamp", "TextureRamp", ProducedType = typeof(TexRampImportData))]
        private static void CreateScenesCollection(string atPath)
        {
            ProjectWindowUtil.CreateAssetWithContent("TexRamp." + TexRampImporter.EXT, 
                TexureRampSerialization.SerializeToString(TextureRamp.Default));
        }
    }
}