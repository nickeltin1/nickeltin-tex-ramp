using System;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace nickeltin.TexRamp.Editor
{
    /// <summary>
    /// TODO: Sprite support
    /// TODO: public static context api to generate unity assets from *.texramp and metadata
    /// </summary>
    [ScriptedImporter(VERSION, EXT)]
    internal class TexRampImporter : ScriptedImporter
    {
        public enum Compression
        {
            Disabled,
            HighQuality,
            LowQuality
        }
        
        public const string EXT = "texramp";
        public const int VERSION = 2;
        
        [SerializeField] internal int _majorResolution = 256;
        [SerializeField] internal int _minorResolution = 32;
        [SerializeField] internal Orientation _orientation = Orientation.Horizontal;
        [SerializeField] internal TextureWrapMode _wrapMode = TextureWrapMode.Repeat;
        [SerializeField] internal FilterMode _filterMode = FilterMode.Bilinear;
        [SerializeField] internal Compression _compression = Compression.HighQuality;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var w = _majorResolution;
            var h = 1;

            if (_orientation == Orientation.Vertical)
            {
                w = 1;
                h = _majorResolution;
            }

            TextureFormat texFormat;
            var ramp = TexureRampSerialization.Load(assetPath);
            switch (ramp.Format)
            {
                case Format.SingleChannel:
                    texFormat = ramp.UseAlphaChannel ? TextureFormat.Alpha8 : TextureFormat.R8;
                    break;
                case Format.RG:
                    texFormat = TextureFormat.RG16;
                    break;
                case Format.RGB:
                    texFormat = TextureFormat.RGB24;
                    break;
                case Format.RGBA:
                case Format.Gradient:
                    texFormat = TextureFormat.RGBA32;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var oneDTex = new Texture2D(w,h, texFormat, false)
            {
                wrapMode = _wrapMode,
                filterMode = _filterMode
            };
            
            var pixels = new Color32[_majorResolution];
            var step = 1f / (_majorResolution - 1);
            for (var i = 0; i < _majorResolution; i++)
            {
                pixels[i] = ramp.Evaluate(step * i);
            }

            oneDTex.SetPixels32(pixels);
            oneDTex.Apply();
            
            w = _majorResolution;
            h = _minorResolution;

            if (_orientation == Orientation.Vertical)
            {
                w = _minorResolution;
                h = _majorResolution;
            }

            var twoDTex= Blit(oneDTex, w, h);

            var notCompressable = ramp.Format == Format.SingleChannel;
            if (!notCompressable)
            {
                switch (_compression)
                {
                    case Compression.Disabled:
                        break;
                    case Compression.HighQuality:
                        twoDTex.Compress(true);
                        break;
                    case Compression.LowQuality:
                        twoDTex.Compress(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            twoDTex.Apply();
            DestroyImmediate(oneDTex);


            twoDTex.name = Path.GetFileNameWithoutExtension(assetPath) + "(Generated)";
            ctx.AddObjectToAsset("TexRamp", twoDTex);
            ctx.SetMainObject(twoDTex);
        }

        public static Texture2D Blit(Texture2D tex, int newWidth, int newHeight)
        {
            return Blit(tex, newWidth, newHeight, tex.format);
        }
        
        public static Texture2D Blit(Texture2D tex, int newWidth, int newHeight, TextureFormat format)
        {
            var blitTex = new Texture2D(newWidth, newHeight, format, false)
            {
                wrapMode = tex.wrapMode,
                filterMode = tex.filterMode,
            };
            
            if (tex.format == TextureFormat.RGBA32)
            {
                blitTex.alphaIsTransparency = true;
            }

            var rtDesc = new RenderTextureDescriptor(newWidth, newHeight, GraphicsFormat.R8G8B8A8_UNorm, 0, 0);
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