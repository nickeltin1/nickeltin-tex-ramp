using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    public abstract class ShapeImporter
    {
        internal ShapeImporter()
        {
            
        }

        public abstract Texture2D GenerateTexture(ShapeAssetImporter mainImporter, TextureShape shape, TextureFormat texFormat);
    }
}