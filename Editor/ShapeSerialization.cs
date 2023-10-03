using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    internal static class ShapeSerialization
    {
        /// <summary>
        /// Loads all assets at path and returns first <see cref="ShapeContainer"/>.
        /// Its <see cref="ShapeContainer.Shape"/> will contain associated shape data.
        /// </summary>
        public static ShapeContainer Load(string path)
        {
            return InternalEditorUtility.LoadSerializedFileAndForget(path).OfType<ShapeContainer>().First();
        }

        /// <inheritdoc cref="Load(string)"/>
        /// <remarks>
        ///     Fills provided container.
        ///     Loads container instance regularly and than just copies it to provided container,
        ///     loaded instance destroyed afterwards. 
        /// </remarks>
        public static void Load(string path, ShapeContainer container)
        {
            var loadedInstance = Load(path);
            EditorUtility.CopySerialized(loadedInstance, container);
            Object.DestroyImmediate(loadedInstance);
        }
        
        public static byte[] Serialize(ShapeContainer container)
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();
            Save(tempPath, container);
            var bytes = File.ReadAllBytes(tempPath);
            File.Delete(tempPath);
            return bytes;
        }
        
        public static string SerializeToString(ShapeContainer container)
        {
            return Encoding.UTF8.GetString(Serialize(container));
        }

        public static byte[] Serialize(TextureShape data)
        {
            var container = data.Wrap();
            var bytes = Serialize(container);
            Object.DestroyImmediate(container);
            return bytes;
        }

        
        public static string SerializeToString(TextureShape data)
        {
            return Encoding.UTF8.GetString(Serialize(data));
        }
        
        public static void Save(string path, ShapeContainer container)
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]{container}, path, true);
        }
    }
}