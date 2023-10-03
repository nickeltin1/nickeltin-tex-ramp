using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    internal class ShapeContainer : ScriptableObject
    {
        [SerializeReference] public TextureShape Shape = null;
    }
}