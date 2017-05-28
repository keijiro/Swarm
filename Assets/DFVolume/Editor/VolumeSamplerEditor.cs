// DFVolume - Distance field volume generator for Unity
// https://github.com/keijiro/DFVolume

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DFVolume
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VolumeSampler))]
    class VolumeSamplerEditor : Editor
    {
        SerializedProperty _resolution;
        SerializedProperty _extent;

        void OnEnable()
        {
            _resolution = serializedObject.FindProperty("_resolution");
            _extent = serializedObject.FindProperty("_extent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_resolution);
            EditorGUILayout.PropertyField(_extent);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Create Volume Data")) CreateVolumeData();

            CheckSkewedTransform();
        }

        void CreateVolumeData()
        {
            var output = new List<Object>();

            foreach (VolumeSampler sampler in targets)
            {
                var path = "Assets/New Volume Data.asset";
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                var asset = ScriptableObject.CreateInstance<VolumeData>();
                asset.Initialize(sampler);

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.AddObjectToAsset(asset.texture, asset);
            }

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.objects = output.ToArray();
        }

        void CheckSkewedTransform()
        {
            if (targets.Any(o => ((Component)o).transform.lossyScale != Vector3.one))
                EditorGUILayout.HelpBox(
                    "Using scale in transform may introduce error in output volumes.",
                    MessageType.Warning
                );
        }
    }
}
