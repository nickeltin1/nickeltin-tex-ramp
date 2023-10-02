using System.IO;
using System.Text;
using UnityEngine;

namespace nickeltin.TexRamp.Editor
{
    public static class TexureRampSerialization
    {
        public static byte[] Serialize(TextureRamp ramp)
        {
            var str = JsonUtility.ToJson(ramp, true);
            return Encoding.UTF8.GetBytes(str);
        }
        
        public static string SerializeToString(TextureRamp ramp)
        {
            return Encoding.UTF8.GetString(Serialize(ramp));
        }


        public static TextureRamp Deserialize(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            var inst = (object)TextureRamp.Default;
            
            //TODO: check is this works for boxed struct
            JsonUtility.FromJsonOverwrite(str, inst);
            return (TextureRamp)inst;
        }


        public static TextureRamp Load(string path)
        {
            return Deserialize(File.ReadAllBytes(path));
        }

        public static void Save(string path, TextureRamp ramp)
        {
            File.WriteAllBytes(path, Serialize(ramp));
        }
    }
}