﻿using System;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace nickeltin.TextureShapes.Editor
{
    [ScriptedImporter(VERSION, EXT)]
    public class ShapeAssetImporter : ScriptedImporter 
    {
        public const string EXT = "texshape";
        public const int VERSION = 0;
        
        public enum Compression
        {
            Disabled,
            HighQuality,
            LowQuality
        }
        
        [SerializeField] internal int _majorResolution = 512;
        [SerializeField] internal int _minorResolution = 512;
        [SerializeField] internal TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
        [SerializeField] internal FilterMode _filterMode = FilterMode.Bilinear;
        [SerializeField] internal Compression _compression = Compression.Disabled;
        [SerializeField] internal bool _generateSprite = false;
        [SerializeField] internal int _pixelsPerUnit = 100;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var container = ShapeSerialization.Load(assetPath);
            
            TextureFormat texFormat;
            var shape = container.Shape;
            switch (shape.Format)
            {
                case ShapeFormat.SingleChannel:
                    texFormat = shape.UseAlphaChannel ? TextureFormat.Alpha8 : TextureFormat.R8;
                    break;
                case ShapeFormat.RG:
                    texFormat = TextureFormat.RG16;
                    break;
                case ShapeFormat.RGB:
                    texFormat = TextureFormat.RGB24;
                    break;
                case ShapeFormat.RGBA:
                case ShapeFormat.Gradient:
                    texFormat = TextureFormat.RGBA32;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var importer = container.Shape.GetImporter();
            var tex = importer.GenerateTexture(this, shape, texFormat);
            
            Compress(tex, shape);
            
            if (texFormat == TextureFormat.RGBA32 || (texFormat == TextureFormat.Alpha8))
            {
                tex.alphaIsTransparency = true;
            }
            
            var texName = Path.GetFileNameWithoutExtension(assetPath);
            
            if (_generateSprite)
            {
                var sprite = Sprite.Create(tex, Rect.MinMaxRect(0, 0, tex.width, tex.height), 
                    new Vector2(0.5f, 0.5f),
                    _pixelsPerUnit, 0, SpriteMeshType.FullRect);
                sprite.name = texName + "(Sprite)";
                ctx.AddObjectToAsset("TexRampSprite", sprite);
            }
            
            
            tex.name = texName + "(Generated)";
            ctx.AddObjectToAsset("TextureShape", tex);
            ctx.SetMainObject(tex);
        }

        public void Compress(Texture2D tex, TextureShape shape)
        {
            var notCompressable = shape.Format == ShapeFormat.SingleChannel;
            if (!notCompressable)
            {
                switch (_compression)
                {
                    case Compression.Disabled:
                        break;
                    case Compression.HighQuality:
                        tex.Compress(true);
                        break;
                    case Compression.LowQuality:
                        tex.Compress(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public static Texture2D Blit(Texture2D tex, int newWidth, int newHeight)
        {
            return Blit(tex, newWidth, newHeight, tex.format);
        }
        
        public static Texture2D Blit(Texture2D tex, int newWidth, int newHeight, TextureFormat format)
        {
            var blitTex = new Texture2D(newWidth, newHeight, format, false, true)
            {
                wrapMode = tex.wrapMode,
                filterMode = tex.filterMode,
            };

            var rtDesc = new RenderTextureDescriptor(newWidth, newHeight, GraphicsFormat.R8G8B8A8_UNorm, 0, 0)
            {
                sRGB = false, 
            };
            
            var rt = RenderTexture.GetTemporary(rtDesc);
            rt.wrapMode = tex.wrapMode;
            rt.filterMode = tex.filterMode;
            
            // Just in case blitting with black tex
            Graphics.Blit(Texture2D.blackTexture, rt);
            Graphics.Blit(tex, rt);
            RenderTexture.active = rt;
            blitTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            
            return blitTex;
        }
    }
}