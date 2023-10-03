using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace nickeltin.TextureShapes.Editor
{
    internal class RampImporter : ShapeImporter
    {
        public override void OnImportAsset(ShapeAssetImporter mainImporter, AssetImportContext ctx, 
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

            var twoDTex = ShapeAssetImporter.Blit(oneDTex, width, height);
            
            mainImporter.Compress(twoDTex, shape);
            
            twoDTex.Apply();
            Object.DestroyImmediate(oneDTex);
            
            var texName = Path.GetFileNameWithoutExtension(mainImporter.assetPath);
            
            if (mainImporter._generateSprite)
            {
                var sprite = Sprite.Create(twoDTex, Rect.MinMaxRect(0, 0, width, height), 
                    new Vector2(0.5f, 0.5f),
                    mainImporter._pixelsPerUnit, 0, SpriteMeshType.FullRect);
                sprite.name = texName + "(Sprite)";
                ctx.AddObjectToAsset("TexRampSprite", sprite);
            }

            twoDTex.name = texName + "(Generated)";
            ctx.AddObjectToAsset("TexRamp", twoDTex);
            ctx.SetMainObject(twoDTex);
        }
    }
}