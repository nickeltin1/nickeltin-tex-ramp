using System;
using UnityEngine;

namespace nickeltin.TexRamp.Editor
{
    /// <summary>
    /// Serializable wrapper with texture curves and other settings to generate texture from it
    /// </summary>
    [Serializable]
    public struct TextureRamp
    {
        public Format Format;
        public bool UseAlphaChannel;
        public Gradient Gradient;
        public TextureChannel RChannel;
        public TextureChannel GChannel;
        public TextureChannel BChannel;
        public TextureChannel AChannel;

        public TextureRamp(Format format,
            Gradient gradient, bool useAlphaChannel,
            TextureChannel rChannel, TextureChannel gChannel, 
            TextureChannel bChannel, TextureChannel aChannel)
        {
            Format = format;
            Gradient = gradient;
            RChannel = rChannel;
            GChannel = gChannel;
            BChannel = bChannel;
            AChannel = aChannel;

            UseAlphaChannel = useAlphaChannel;
        }

        public static TextureRamp Default
        {
            get
            {
                var grad = new Gradient();
                grad.SetKeys(new []
                {
                    new GradientColorKey(Color.white, 0), 
                    new GradientColorKey(Color.white, 0.5f), 
                    new GradientColorKey(Color.white, 1)
                    
                }, new []
                {
                    new GradientAlphaKey(0, 0),
                    new GradientAlphaKey(1, 0.5f),
                    new GradientAlphaKey(0, 1),
                });
                
                return new TextureRamp(Format.SingleChannel, grad, false,
                    TextureChannel.Default, TextureChannel.Default,
                    TextureChannel.Default, TextureChannel.Default);
            }
        }


        public readonly Color Evaluate(float t)
        {
            t = Mathf.Clamp01(t);
            if (Format == Format.Gradient)
            {
                return Gradient.Evaluate(t);
            }

            float EvalChannel(TextureChannel channel)
            {
                var tL = t;
                if (channel.Inverted) tL = 1 - tL;
                return channel.Curve.Evaluate(tL);
            }

            var r = EvalChannel(RChannel);
            var g = EvalChannel(GChannel);
            var b = EvalChannel(BChannel);
            var a = EvalChannel(AChannel);

            return new Color(r, g, b, a);
        }
    }

    [Serializable]
    public struct TextureChannel
    {
        public AnimationCurve Curve;
        public bool Inverted;

        public TextureChannel(AnimationCurve curve, bool inverted)
        {
            Curve = curve;
            Inverted = inverted;
        }

        public static TextureChannel Default
        {
            get
            {
                return new TextureChannel(new AnimationCurve(new []
                {
                    new Keyframe(0, 0),
                    new Keyframe(0.5f, 1),
                    new Keyframe(1, 0)
                }), false);
            }
        }
    }
    
    public enum Orientation
    {
        Horizontal, Vertical
    }
    
    public enum Format
    {
        SingleChannel,
        RG,
        RGB,
        RGBA,
        Gradient
    }
}