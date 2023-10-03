using UnityEditor;

namespace nickeltin.TextureShapes.Editor
{
    internal class RampDrawer : ShapeDrawer
    {
        public override string GetShapeDisplayName()
        {
            return "Ramp\nOne-dimensional texture with control over channels, created either with curves or gradient.";
        }

        public override void OnShapeGUI(SerializedProperty property)
        {
            var format = property.FindPropertyRelative(nameof(RampShape.Format));
            var gradient = property.FindPropertyRelative(nameof(RampShape.Gradient));
            var useAlphaChannel = property.FindPropertyRelative(nameof(RampShape.UseAlphaChannel));
            var rChannel = property.FindPropertyRelative(nameof(RampShape.RChannel));
            var gChannel = property.FindPropertyRelative(nameof(RampShape.GChannel));
            var bChannel = property.FindPropertyRelative(nameof(RampShape.BChannel));
            var aChannel = property.FindPropertyRelative(nameof(RampShape.AChannel));
            var orientation = property.FindPropertyRelative(nameof(RampShape.Orientation));

            
            var channels = new[] { rChannel, gChannel, bChannel, aChannel };
            EditorGUILayout.PropertyField(orientation);
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
                    DrawChannel(useAlphaChannel.boolValue ? aChannel : rChannel);
                }
                else
                {
                    for (var i = 0; i <= format.enumValueIndex; i++)
                    {
                        DrawChannel(channels[i]);
                    }
                }
            }
        }
        
        public static void DrawChannel(SerializedProperty property)
        {
            var curve = property.FindPropertyRelative(nameof(RampShape.Channel.Curve));
            var inverted = property.FindPropertyRelative(nameof(RampShape.Channel.Inverted));
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField(property.displayName, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.PropertyField(curve);
                EditorGUILayout.PropertyField(inverted);
            }
        }
    }
}