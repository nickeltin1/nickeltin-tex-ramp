using System;
using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    [Serializable]
    public class RampShape : TextureShape
    {
        [Serializable]
        public struct Channel
        {
            public AnimationCurve Curve;
            public bool Inverted;

            public Channel(AnimationCurve curve, bool inverted)
            {
                Curve = curve;
                Inverted = inverted;
            }

            public static Channel Bell => new(TextureShapeUtil.BellCurve(), false);
            public static Channel LeftSteep => new(TextureShapeUtil.LeftSteep(), false);
        }
        
        public override Type GetImporterType() => typeof(RampImporter);

        public override Type GetDrawerType() => typeof(RampDrawer);

        public ShapeOrientation Orientation = ShapeOrientation.Horizontal;
        public Gradient Gradient = new Gradient();
        public Channel RChannel = Channel.Bell;
        public Channel GChannel = Channel.Bell;
        public Channel BChannel = Channel.Bell;
        public Channel AChannel = Channel.Bell;
        
        public Color Evaluate(float t)
        {
            t = Mathf.Clamp01(t);
            if (Format == ShapeFormat.Gradient)
            {
                return Gradient.Evaluate(t);
            }

            float EvalChannel(Channel channel)
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
}