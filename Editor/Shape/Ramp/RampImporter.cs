using UnityEngine;
using Object = UnityEngine.Object;

namespace nickeltin.TextureShapes.Editor
{
    internal class RampImporter : ShapeImporter
    {
        public override Texture2D GenerateTexture(ShapeAssetImporter mainImporter,
            TextureShape shape, TextureFormat texFormat)
        {
            var majorResolution = mainImporter._majorResolution;
            var minorResolution = mainImporter._minorResolution;
            var ramp = (RampShape)shape;
            
            var width = majorResolution;
            var height = 1;

            if (ramp.Orientation == ShapeOrientation.Vertical)
            {
                width = 1;
                height = majorResolution;
            }
            
            var oneDTex = new Texture2D(width, height, texFormat, false, true)
            {
                wrapMode = mainImporter._wrapMode,
                filterMode = mainImporter._filterMode
            };
            
            var pixels = new Color32[majorResolution];
            var step = 1f / (majorResolution - 1);
            for (var i = 0; i < majorResolution; i++)
            {
                pixels[i] = ramp.Evaluate(step * i);
            }

            oneDTex.SetPixels32(pixels);
            oneDTex.Apply();
            
            width = majorResolution;
            height = minorResolution;

            if (ramp.Orientation == ShapeOrientation.Vertical)
            {
                width = minorResolution;
                height = majorResolution;
            }

            // Blitting to increase resolution in one dimension,
            // because we actually generating single pixels line but need 2D texture.
            var twoDTex = ShapeAssetImporter.Blit(oneDTex, width, height);
            
            twoDTex.Apply();
            Object.DestroyImmediate(oneDTex);

            return twoDTex;
        }
    }
}