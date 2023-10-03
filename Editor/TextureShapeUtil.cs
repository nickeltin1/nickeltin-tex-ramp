using System;
using System.Collections.Generic;
using UnityEngine;

namespace nickeltin.TextureShapes.Editor
{
    public static class TextureShapeUtil
    {
        private static readonly Dictionary<Type, ShapeDrawer> _drawers;
        private static readonly Dictionary<Type, ShapeImporter> _importers;

        static TextureShapeUtil()
        {
            _importers = new Dictionary<Type, ShapeImporter>();
            _drawers = new Dictionary<Type, ShapeDrawer>();
        }

        public static ShapeImporter GetImporter(this TextureShape forShape)
        {
            var type = forShape.GetType();
            var importerType = forShape.GetImporterType();
            if (importerType == null)
                throw new ArgumentNullException();

            if (!_importers.TryGetValue(type, out var importer))
            {
                importer = (ShapeImporter)Activator.CreateInstance(importerType);
                _importers.Add(type, importer);
            }

            return importer;
        }

        public static ShapeDrawer GetDrawer(this TextureShape forShape)
        {
            var type = forShape.GetType();
            var drawerType = forShape.GetDrawerType();
            
            if (drawerType == null)
                throw new ArgumentNullException();

            if (!_drawers.TryGetValue(type, out var drawer))
            {
                drawer = (ShapeDrawer)Activator.CreateInstance(drawerType);
                _drawers.Add(type, drawer);
            }

            return drawer;
        }

        /// <summary>
        /// Creates container over texture shape used for serialization
        /// </summary>
        /// <returns></returns>
        internal static ShapeContainer Wrap(this TextureShape data)
        {
            var instance = ScriptableObject.CreateInstance<ShapeContainer>();
            instance.Shape = data;
            return instance;
        }

        internal static AnimationCurve BellCurve()
        {
            return new AnimationCurve(new[]
            {
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            });
        }
        
        internal static AnimationCurve LeftSteep()
        {
            return new AnimationCurve(new[]
            {
                new Keyframe(0, 0),
                new Keyframe(0.01f, 1),
                new Keyframe(1, 1)
            });
        }
    }
}