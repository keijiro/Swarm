// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
// https://github.com/keijiro/Swarm

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Swarm
{
    // Custom inspector for TubeTemplate.
    [CustomEditor(typeof(TubeTemplate)), CanEditMultipleObjects]
    public sealed class TubeTemplateEditor : Editor
    {
        #region Custom inspector

        SerializedProperty _divisions;
        SerializedProperty _segments;

        void OnEnable()
        {
            _divisions = serializedObject.FindProperty("_divisions");
            _segments = serializedObject.FindProperty("_segments");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_divisions);
            EditorGUILayout.PropertyField(_segments);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            // Rebuild the templates when there are changes on the properties.
            if (rebuild) foreach (TubeTemplate t in targets) t.Rebuild();
        }

        #endregion

        #region Custom menu items

        [MenuItem("Assets/Create/Swarm/Tube Template")]
        static void CreteTubeTemplate()
        {
            // Determine the destination path.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(path), "");
            path += "/New Tube Template.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            // Create a tube template asset.
            var asset = ScriptableObject.CreateInstance<TubeTemplate>();
            asset.Rebuild();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.AddObjectToAsset(asset.mesh, asset);

            // Save the generated mesh asset.
            AssetDatabase.SaveAssets();

            // Select the generated asset on the project view.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        #endregion
    }
}
