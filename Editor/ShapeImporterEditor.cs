using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace nickeltin.TextureShapes.Editor
{
    [CustomEditor(typeof(ShapeAssetImporter)), CanEditMultipleObjects]
    internal class ShapeImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _majorResolution;
        private SerializedProperty _minorResolution;
        private SerializedProperty _compression;
        
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _format;
        private SerializedProperty _useAlphaChannel;
        private SerializedProperty _generateSprite;
        private SerializedProperty _pixelsPerUnit;

        private SerializedProperty _shape;
        private ShapeDrawer _drawer;
        private bool _hasDifferentShapesSelected;

        protected override bool ShouldHideOpenButton() => true;

        public override bool showImportedObject => true;

        protected override Type extraDataType => typeof(ShapeContainer);

        protected override void InitializeExtraDataInstance(Object extraData, int targetIndex)
        {
            var importer = (ShapeAssetImporter)targets[targetIndex];
            // Checking is ramp file actually exist because it might be called when it was deleted
            if (File.Exists(importer.assetPath))
            {   
                var container = (ShapeContainer)extraData;
                ShapeSerialization.Load(importer.assetPath, container);
                var drawer = container.Shape.GetDrawer();
                if (_drawer != null && _drawer != drawer)
                {
                    _hasDifferentShapesSelected = true;
                }
                else
                {
                    _drawer = drawer;
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _shape = extraDataSerializedObject.FindProperty(nameof(ShapeContainer.Shape));
            _majorResolution = serializedObject.FindProperty(nameof(ShapeAssetImporter._majorResolution));
            _minorResolution = serializedObject.FindProperty(nameof(ShapeAssetImporter._minorResolution));
            _compression = serializedObject.FindProperty(nameof(ShapeAssetImporter._compression));
            _filterMode = serializedObject.FindProperty(nameof(ShapeAssetImporter._filterMode));
            _wrapMode = serializedObject.FindProperty(nameof(ShapeAssetImporter._wrapMode));
            
            _generateSprite = serializedObject.FindProperty(nameof(ShapeAssetImporter._generateSprite));
            _pixelsPerUnit = serializedObject.FindProperty(nameof(ShapeAssetImporter._pixelsPerUnit));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            extraDataSerializedObject.UpdateIfRequiredOrScript();

            DrawDimensionField(_majorResolution);
            DrawDimensionField(_minorResolution);
            
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_filterMode);
            EditorGUILayout.PropertyField(_compression);
            EditorGUILayout.PropertyField(_generateSprite);
            if (_generateSprite.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    var pixelsPerUnit = EditorGUILayout.IntField(
                        EditorGUIUtility.TrTextContent(_pixelsPerUnit.displayName, _pixelsPerUnit.tooltip), 
                        _pixelsPerUnit.intValue);

                    pixelsPerUnit = Mathf.Clamp(pixelsPerUnit, 1, int.MaxValue);
                    _pixelsPerUnit.intValue = pixelsPerUnit;
                }
            }
            
            
            EditorGUILayout.Space();
            
            ShapeGUI();
            
            extraDataSerializedObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            
            ApplyRevertGUI();
            
            EditorGUILayout.Space();

            ExtractTextureGUI();
        }

        private void ShapeGUI()
        {
            if (_hasDifferentShapesSelected)
            {
                EditorGUILayout.HelpBox(
                    "There are shapes with different type selected, select shapes with same type to multi-edit", 
                    MessageType.Warning);
                return;
            }
            
            EditorGUILayout.HelpBox(_drawer.GetShapeDisplayName(), MessageType.Info);
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.BeginProperty(rect, GUIContent.none, _shape);
            
            _drawer.OnShapeGUI(_shape);
            
            EditorGUI.EndProperty();
            EditorGUILayout.EndVertical();
        }

        private void ExtractTextureGUI()
        {
            using (new EditorGUI.DisabledScope(HasModified()))
            {
                if (GUILayout.Button(serializedObject.isEditingMultipleObjects 
                        ? "Extract textures" 
                        : "Extract texture"))
                {
                    for (var i = 0; i < targets.Length; i++)
                    {
                        var importer = (ShapeAssetImporter)targets[i];
                        var path = importer.assetPath;
                        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                        path = Path.ChangeExtension(path, "png");
                        path = AssetDatabase.GenerateUniqueAssetPath(path);
                        
                        // Blitting texture because it might be compressed and can't be encoded to png
                        var tempTex = ShapeAssetImporter.Blit(tex, tex.width, tex.height, TextureFormat.RGBA32);
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
                var container = (ShapeContainer)extraDataTargets[i];
                var importer = (ShapeAssetImporter)targets[i];
                ShapeSerialization.Save(importer.assetPath, container);
            }
        }
        

        public static void DrawDimensionField(SerializedProperty property)
        {
            var value = EditorGUILayout.IntSlider(EditorGUIUtility.TrTextContent(property.displayName, property.tooltip),
                property.intValue, 8, 1024);
            value = (value + 3) & ~3;
            property.intValue = value;
        }
    }
}