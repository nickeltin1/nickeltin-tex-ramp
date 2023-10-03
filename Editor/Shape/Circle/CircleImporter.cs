using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    /// <summary>
    /// TODO: figure out AA
    /// </summary>
    internal class CircleImporter : ShapeImporter
    {
        public override Texture2D GenerateTexture(ShapeAssetImporter mainImporter, TextureShape shape, TextureFormat texFormat)
        {
            var textureWidth = mainImporter._majorResolution;
            var textureHeight = mainImporter._minorResolution;
            var circle = (CircleShape)shape;
            
            
            var smallerDimension = Mathf.Min(textureWidth, textureHeight);
            var tex = new Texture2D(textureWidth, textureHeight, texFormat, false, true)
            {
                wrapMode = mainImporter._wrapMode,
                filterMode = mainImporter._filterMode
            };
            
            var pixels = new Color32[textureWidth * textureHeight];

            var circleCenter = new Vector2(textureWidth / 2f, textureHeight / 2f);
            var circleRadius = smallerDimension / 2f;

            for (var x = 0; x < textureWidth; x++)
            {
                for (var y = 0; y < textureHeight; y++)
                {
                    var pixelPos = new Vector2(x, y);
                    var distance = Vector2.Distance(pixelPos, circleCenter) / circleRadius;
                    var normalizedDistance = 1f - Mathf.Clamp01(distance);
                    pixels[y * textureWidth + x] = circle.Evaluate(normalizedDistance);
                }
            }
            
            
            tex.SetPixels32(pixels);
            tex.Apply();

            if (texFormat == TextureFormat.RGBA32)
            {
                tex.alphaIsTransparency = true;
            }
            
            return tex;
        }
    }
}