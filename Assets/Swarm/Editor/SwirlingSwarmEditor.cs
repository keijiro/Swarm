// Swarm - Special renderer that draws a swarm of wobbling/crawling tubes.
// https://github.com/keijiro/Swarm

using UnityEngine;
using UnityEditor;

namespace Swarm
{
    // Custom inspector for SwirlingSwarm
    [CustomEditor(typeof(SwirlingSwarm)), CanEditMultipleObjects]
    public class SwirlingSwarmEditor : Editor
    {
        SerializedProperty _instanceCount;

        SerializedProperty _template;
        SerializedProperty _radius;
        SerializedProperty _material;
        SerializedProperty _gradient;

        SerializedProperty _spread;
        SerializedProperty _length;
        SerializedProperty _noiseFrequency;
        SerializedProperty _noiseMotion;

        void OnEnable()
        {
            _instanceCount = serializedObject.FindProperty("_instanceCount");

            _template = serializedObject.FindProperty("_template");
            _radius = serializedObject.FindProperty("_radius");
            _material = serializedObject.FindProperty("_material");
            _gradient = serializedObject.FindProperty("_gradient");

            _spread = serializedObject.FindProperty("_spread");
            _length = serializedObject.FindProperty("_length");
            _noiseFrequency = serializedObject.FindProperty("_noiseFrequency");
            _noiseMotion = serializedObject.FindProperty("_noiseMotion");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_instanceCount);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_template);
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_material);
            EditorGUILayout.PropertyField(_gradient);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_spread);
            EditorGUILayout.PropertyField(_length);
            EditorGUILayout.PropertyField(_noiseFrequency);
            EditorGUILayout.PropertyField(_noiseMotion);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
