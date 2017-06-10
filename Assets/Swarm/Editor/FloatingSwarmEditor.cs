// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
// https://github.com/keijiro/Swarm

using UnityEngine;
using UnityEditor;

namespace Swarm
{
    // Custom inspector for FloatingSwarm
    [CustomEditor(typeof(FloatingSwarm)), CanEditMultipleObjects]
    public class FloatingSwarmEditor : Editor
    {
        SerializedProperty _instanceCount;
        SerializedProperty _template;
        SerializedProperty _radius;
        SerializedProperty _trim;

        SerializedProperty _attractor;
        SerializedProperty _attractorPosition;
        SerializedProperty _attractorSpread;
        SerializedProperty _attractorForce;

        SerializedProperty _forceRandomness;
        SerializedProperty _drag;

        SerializedProperty _headNoiseForce;
        SerializedProperty _headNoiseFrequency;
        SerializedProperty _trailNoiseVelocity;
        SerializedProperty _trailNoiseFrequency;
        SerializedProperty _noiseSpread;
        SerializedProperty _noiseMotion;

        SerializedProperty _material;
        SerializedProperty _gradient;

        SerializedProperty _randomSeed;

        static class Labels
        {
            public static GUIContent force = new GUIContent("Force");
            public static GUIContent frequency = new GUIContent("Frequency");
            public static GUIContent headForce = new GUIContent("Force (head)");
            public static GUIContent headFrequency = new GUIContent("Frequency (head)");
            public static GUIContent motion = new GUIContent("Motion");
            public static GUIContent position = new GUIContent("Position");
            public static GUIContent randomness = new GUIContent("Randomness");
            public static GUIContent spread = new GUIContent("Spread");
            public static GUIContent trailFrequency = new GUIContent("Frequency (trail)");
            public static GUIContent trailVelocity = new GUIContent("Velocity (trail)");
        }

        void OnEnable()
        {
            _instanceCount = serializedObject.FindProperty("_instanceCount");
            _template = serializedObject.FindProperty("_template");
            _radius = serializedObject.FindProperty("_radius");
            _trim = serializedObject.FindProperty("_trim");

            _attractor = serializedObject.FindProperty("_attractor");
            _attractorPosition = serializedObject.FindProperty("_attractorPosition");
            _attractorSpread = serializedObject.FindProperty("_attractorSpread");
            _attractorForce = serializedObject.FindProperty("_attractorForce");

            _forceRandomness = serializedObject.FindProperty("_forceRandomness");
            _drag = serializedObject.FindProperty("_drag");

            _headNoiseForce = serializedObject.FindProperty("_headNoiseForce");
            _headNoiseFrequency = serializedObject.FindProperty("_headNoiseFrequency");
            _trailNoiseVelocity = serializedObject.FindProperty("_trailNoiseVelocity");
            _trailNoiseFrequency = serializedObject.FindProperty("_trailNoiseFrequency");
            _noiseSpread = serializedObject.FindProperty("_noiseSpread");
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
            EditorGUILayout.PropertyField(_trim);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_attractor);
            EditorGUI.indentLevel++;
            if (_attractor.objectReferenceValue == null)
                EditorGUILayout.PropertyField(_attractorPosition, Labels.position);
            EditorGUILayout.PropertyField(_attractorSpread, Labels.spread);
            EditorGUILayout.PropertyField(_attractorForce, Labels.force);
            EditorGUILayout.PropertyField(_forceRandomness, Labels.randomness);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(_drag);

            EditorGUILayout.LabelField("Noise Field");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_headNoiseForce, Labels.headForce);
            EditorGUILayout.PropertyField(_headNoiseFrequency, Labels.headFrequency);
            EditorGUILayout.PropertyField(_trailNoiseVelocity, Labels.trailVelocity);
            EditorGUILayout.PropertyField(_trailNoiseFrequency, Labels.trailFrequency);
            EditorGUILayout.PropertyField(_noiseSpread, Labels.spread);
            EditorGUILayout.PropertyField(_noiseMotion, Labels.motion);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_material);
            EditorGUILayout.PropertyField(_gradient);
            EditorGUILayout.PropertyField(_randomSeed);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && GUILayout.Button("Reset"))
                foreach (FloatingSwarm fs in targets) fs.ResetPositions();
        }
    }
}
