using System;
using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    [Serializable]
    public class CircleShape : TextureShape
    {
        public override Type GetImporterType() => typeof(CircleImporter);

        public override Type GetDrawerType() => typeof(CircleDrawer);
        
        
        public Gradient Gradient = new Gradient();
        public RampShape.Channel RChannel = RampShape.Channel.LeftSteep;
        public RampShape.Channel GChannel = RampShape.Channel.LeftSteep;
        public RampShape.Channel BChannel = RampShape.Channel.LeftSteep;
        public RampShape.Channel AChannel = RampShape.Channel.LeftSteep;
        
        public Color Evaluate(float t)
        {
            t = Mathf.Clamp01(t);
            
            if (Format == ShapeFormat.Gradient)
            {
                return Gradient.Evaluate(t);
            }

            float EvalChannel(RampShape.Channel channel)
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