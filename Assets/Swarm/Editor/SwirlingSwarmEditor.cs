// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
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
        SerializedProperty _length;

        SerializedProperty _spread;
        SerializedProperty _noiseFrequency;
        SerializedProperty _noiseMotion;

        SerializedProperty _material;
        SerializedProperty _gradient;

        SerializedProperty _randomSeed;

        static class Labels
        {
            public static GUIContent frequency = new GUIContent("Frequency");
            public static GUIContent motion = new GUIContent("Motion");
        }

        void OnEnable()
        {
            _instanceCount = serializedObject.FindProperty("_instanceCount");
            _template = serializedObject.FindProperty("_template");
            _radius = serializedObject.FindProperty("_radius");
            _length = serializedObject.FindProperty("_length");

            _spread = serializedObject.FindProperty("_spread");
            _noiseFrequency = serializedObject.FindProperty("_noiseFrequency");
            _noiseMotion = serializedObject.FindProperty("_noiseMotion");

            _material = serializedObject.FindProperty("_material");
            _gradient = serializedObject.FindProperty("_gradient");

            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_instanceCount);
            EditorGUILayout.PropertyField(_template);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_length);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_spread);
            EditorGUILayout.LabelField("Noise Field");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_noiseFrequency, Labels.frequency);
            EditorGUILayout.PropertyField(_noiseMotion, Labels.motion);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_material);
            EditorGUILayout.PropertyField(_gradient);

            EditorGUILayout.PropertyField(_randomSeed);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
