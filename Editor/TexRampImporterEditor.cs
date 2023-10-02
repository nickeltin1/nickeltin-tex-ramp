using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace nickeltin.TexRamp.Editor
{
    [CustomEditor(typeof(TexRampImporter)), CanEditMultipleObjects]
    internal class TexRampImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _majorResolution;
        private SerializedProperty _minorResolution;
        private SerializedProperty _compression;
        
        private SerializedProperty _rampProp;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _orientation;
        private SerializedProperty _format;
        private SerializedProperty _gradient;
        private SerializedProperty _useAlphaChannel;
        
        
        private SerializedProperty _rChannel;
        private SerializedProperty _gChannel;
        private SerializedProperty _bChannel;
        private SerializedProperty _aChannel;
        private SerializedProperty[] _channels;


        protected override bool ShouldHideOpenButton() => true;

        public override bool showImportedObject => true;

        protected override Type extraDataType => typeof(TexRampImportData);

        protected override void InitializeExtraDataInstance(Object extraData, int targetIndex)
        {
            var importer = (TexRampImporter)targets[targetIndex];
            // Checking is ramp file actually exist because it might be called when it was deleted
            if (File.Exists(importer.assetPath))
            {   
                var importData = (TexRampImportData)extraData;
                var ramp = TexureRampSerialization.Load(importer.assetPath);
                importData.Ramp = ramp;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            _rampProp = extraDataSerializedObject.FindProperty(nameof(TexRampImportData.Ramp));
            
            
            _majorResolution = serializedObject.FindProperty(nameof(TexRampImporter._majorResolution));
            _minorResolution = serializedObject.FindProperty(nameof(TexRampImporter._minorResolution));
            _compression = serializedObject.FindProperty(nameof(TexRampImporter._compression));
            _filterMode = serializedObject.FindProperty(nameof(TexRampImporter._filterMode));
            _orientation = serializedObject.FindProperty(nameof(TexRampImporter._orientation));
            _wrapMode = serializedObject.FindProperty(nameof(TexRampImporter._wrapMode));
            
            _format = _rampProp.FindPropertyRelative(nameof(TextureRamp.Format));
            _gradient = _rampProp.FindPropertyRelative(nameof(TextureRamp.Gradient));
            
            _useAlphaChannel = _rampProp.FindPropertyRelative(nameof(TextureRamp.UseAlphaChannel));
            
            _rChannel = _rampProp.FindPropertyRelative(nameof(TextureRamp.RChannel));
            _gChannel = _rampProp.FindPropertyRelative(nameof(TextureRamp.GChannel));
            _bChannel = _rampProp.FindPropertyRelative(nameof(TextureRamp.BChannel));
            _aChannel = _rampProp.FindPropertyRelative(nameof(TextureRamp.AChannel));

            _channels = new[] { _rChannel, _gChannel, _bChannel, _aChannel };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            extraDataSerializedObject.UpdateIfRequiredOrScript();

            DrawDimensionField(_majorResolution);
            DrawDimensionField(_minorResolution);
            
            EditorGUILayout.PropertyField(_orientation);
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_filterMode);
            EditorGUILayout.PropertyField(_compression);
            
            EditorGUILayout.Space();
            
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.BeginProperty(rect, GUIContent.none, _rampProp);
            EditorGUILayout.PropertyField(_format);
            
            var channelIndex = _format.enumValueIndex;
            if (channelIndex >= 4)
            {
                EditorGUILayout.PropertyField(_gradient);
            }
            else
            {
                if (channelIndex == 0)
                {
                    EditorGUILayout.PropertyField(_useAlphaChannel);
                    DrawChannel(_useAlphaChannel.boolValue ? _aChannel : _rChannel);
                }
                else
                {
                    for (var i = 0; i <= _format.enumValueIndex; i++)
                    {
                        DrawChannel(_channels[i]);
                    }
                }
            }
            
            EditorGUI.EndProperty();
            EditorGUILayout.EndVertical();
            
            extraDataSerializedObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            
            ApplyRevertGUI();
            
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(HasModified()))
            {
                if (GUILayout.Button(serializedObject.isEditingMultipleObjects 
                        ? "Extract textures" 
                        : "Extract texture"))
                {
                    for (var i = 0; i < targets.Length; i++)
                    {
                        var importer = (TexRampImporter)targets[i];
                        var path = importer.assetPath;
                        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                        path = Path.ChangeExtension(path, "png");
                        path = AssetDatabase.GenerateUniqueAssetPath(path);
                        
                        // Blitting texture because it might be compressed and can't be encoded to png
                        var tempTex = TexRampImporter.Blit(tex, tex.width, tex.height, TextureFormat.RGBA32);
                        File.WriteAllBytes(path, tempTex.EncodeToPNG());
                        DestroyImmediate(tempTex);
                        AssetDatabase.ImportAsset(path);
                        var texImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                        texImporter.sRGBTexture = false;
                        texImporter.SaveAndReimport();
                    }
                }
            }
        }


        protected override void Apply()
        {
            base.Apply();
            for (var i = 0; i < targets.Length; i++)
            {
                var rampImportData = (TexRampImportData)extraDataTargets[i];
                var importer = (TexRampImporter)targets[i];
                TexureRampSerialization.Save(importer.assetPath, rampImportData.Ramp);
            }
        }
        

        private static void DrawDimensionField(SerializedProperty property)
        {
            var value = EditorGUILayout.IntSlider(EditorGUIUtility.TrTextContent(property.displayName, property.tooltip),
                property.intValue, 8, 1024);
            value = (value + 3) & ~3;
            property.intValue = value;
        }
        
        private static void DrawChannel(SerializedProperty property)
        {
            var curve = property.FindPropertyRelative(nameof(TextureChannel.Curve));
            var inverted = property.FindPropertyRelative(nameof(TextureChannel.Inverted));
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField(property.displayName, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.PropertyField(curve);
                EditorGUILayout.PropertyField(inverted);
            }
        }
    }
}