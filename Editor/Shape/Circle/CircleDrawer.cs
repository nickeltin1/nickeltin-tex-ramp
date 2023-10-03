using UnityEditor;

namespace nickeltin.TextureShapes.Editor
{
    public class CircleDrawer : ShapeDrawer
    {
        public override string GetShapeDisplayName()
        {
            return "Circle";
        }

        public override void OnShapeGUI(SerializedProperty property)
        {
            var format = property.FindPropertyRelative(nameof(CircleShape.Format));
            var gradient = property.FindPropertyRelative(nameof(CircleShape.Gradient));
            var useAlphaChannel = property.FindPropertyRelative(nameof(CircleShape.UseAlphaChannel));
            var rChannel = property.FindPropertyRelative(nameof(CircleShape.RChannel));
            var gChannel = property.FindPropertyRelative(nameof(CircleShape.GChannel));
            var bChannel = property.FindPropertyRelative(nameof(CircleShape.BChannel));
            var aChannel = property.FindPropertyRelative(nameof(CircleShape.AChannel));

            
            var channels = new[] { rChannel, gChannel, bChannel, aChannel };
            EditorGUILayout.PropertyField(format);
            
            var channelIndex = format.enumValueIndex;
            if (channelIndex >= 4)
            {
                EditorGUILayout.PropertyField(gradient);
            }
            else
            {
                if (channelIndex == 0)
                {
                    EditorGUILayout.PropertyField(useAlphaChannel);
                    RampDrawer.DrawChannel(useAlphaChannel.boolValue ? aChannel : rChannel);
                }
                else
                {
                    for (var i = 0; i <= format.enumValueIndex; i++)
                    {
                        RampDrawer.DrawChannel(channels[i]);
                    }
                }
            }
        }
    }
}