using UnityEditor.AssetImporters;
using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    public abstract class ShapeImporter
    {
        internal ShapeImporter()
        {
            
        }

        public abstract void OnImportAsset(ShapeAssetImporter mainImporter, AssetImportContext ctx, TextureShape shape,
            TextureFormat texFormat);
    }
}