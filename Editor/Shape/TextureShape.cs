using System;

namespace nickeltin.TextureShapes.Editor
{
    public enum ShapeFormat
    {
        SingleChannel,
        RG,
        RGB,
        RGBA,
        Gradient
    }
    
    public enum ShapeOrientation
    {
        Horizontal, Vertical
    }
    
    /// <summary>
    /// Inherit from this class to create new shape data
    /// </summary>
    [Serializable]
    public abstract class TextureShape
    {
        public ShapeFormat Format = ShapeFormat.SingleChannel;
        public bool UseAlphaChannel = true;
        
        public abstract Type GetImporterType();
        public abstract Type GetDrawerType();
    }
}