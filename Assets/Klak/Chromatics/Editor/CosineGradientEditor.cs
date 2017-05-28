using UnityEditor;
using UnityEngine;

namespace Klak.Chromatics
{
    // Custom editor for cosine gradient object
    [CustomEditor(typeof(CosineGradient))]
    public class CosineGradientEditor : Editor
    {
        GraphDrawer _graph;
        PreviewDrawer _preview;

        SerializedProperty _redCoeffs;
        SerializedProperty _greenCoeffs;
        SerializedProperty _blueCoeffs;

        [SerializeField] Shader _previewShader;

        void OnEnable()
        {
            _graph = new GraphDrawer();
            _preview = new PreviewDrawer(_previewShader);

            _redCoeffs = serializedObject.FindProperty("_redCoeffs");
            _greenCoeffs = serializedObject.FindProperty("_greenCoeffs");
            _blueCoeffs = serializedObject.FindProperty("_blueCoeffs");
        }

        void OnDisable()
        {
            _preview.Cleanup();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _graph.DrawGraph((CosineGradient)target);

            EditorGUILayout.Space();

            _preview.DrawPreview((CosineGradient)target);

            EditorGUILayout.Space();

            ShowSliders("Red", _redCoeffs);
            ShowSliders("Green", _greenCoeffs);
            ShowSliders("Blue", _blueCoeffs);

            serializedObject.ApplyModifiedProperties();
        }

        void ShowSliders(string label, SerializedProperty prop)
        {
            var v = prop.vector4Value;

            EditorGUILayout.LabelField(label);
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel++;
            v.x = EditorGUILayout.Slider("Bias", v.x, 0, 1);
            v.y = EditorGUILayout.Slider("Amplitude", v.y, 0, 1);
            v.z = EditorGUILayout.Slider("Frequency", v.z, 0, 3);
            v.w = EditorGUILayout.Slider("Phase", v.w, 0, 1);
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
                prop.vector4Value = new Vector4(v.x, v.y, v.z, v.w);
        }
    }

    // A utility class for drawing component curves.
    public class GraphDrawer
    {
        #region Public Methods

        public void DrawGraph(CosineGradient grad)
        {
            _rectGraph = GUILayoutUtility.GetRect(128, 80);

            // Background
            DrawRect(0, 0, 1, 1, 0.1f, 0.4f);

            // Horizontal line
            var lineColor = Color.white * 0.4f;
            DrawLine(0, 0.5f, 1, 0.5f, lineColor);

            // Vertical lines
            DrawLine(0.25f, 0, 0.25f, 1, lineColor);
            DrawLine(0.50f, 0, 0.50f, 1, lineColor);
            DrawLine(0.75f, 0, 0.75f, 1, lineColor);

            // R/G/B curves
            DrawGradientCurve(grad.redCoeffs, Color.red);
            DrawGradientCurve(grad.greenCoeffs, Color.green);
            DrawGradientCurve(grad.blueCoeffs, Color.blue);
        }

        void DrawGradientCurve(Vector4 coeffs, Color color)
        {
            for (var i = 0; i < _curveResolution; i++)
            {
                var x = (float)i / (_curveResolution - 1);
                var theta = (coeffs.z * x + coeffs.w) * Mathf.PI * 2;
                var y = coeffs.x + coeffs.y * Mathf.Cos(theta);
                _curveVertices[i] = PointInRect(x, Mathf.Clamp01(y));
            }

            Handles.color = color;
            Handles.DrawAAPolyLine(2.0f, _curveResolution, _curveVertices);
        }

        #endregion

        #region Graph Functions

        // Number of vertices in curve
        const int _curveResolution = 96;

        // Vertex buffers
        Vector3[] _rectVertices = new Vector3[4];
        Vector3[] _lineVertices = new Vector3[2];
        Vector3[] _curveVertices = new Vector3[_curveResolution];

        Rect _rectGraph;

        // Transform a point into the graph rect.
        Vector3 PointInRect(float x, float y)
        {
            x = Mathf.Lerp(_rectGraph.x, _rectGraph.xMax, x);
            y = Mathf.Lerp(_rectGraph.yMax, _rectGraph.y, y);
            return new Vector3(x, y, 0);
        }

        // Draw a line in the graph rect.
        void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            _lineVertices[0] = PointInRect(x1, y1);
            _lineVertices[1] = PointInRect(x2, y2);
            Handles.color = color;
            Handles.DrawAAPolyLine(2.0f, _lineVertices);
        }

        // Draw a rect in the graph rect.
        void DrawRect(float x1, float y1, float x2, float y2, float fill, float line)
        {
            _rectVertices[0] = PointInRect(x1, y1);
            _rectVertices[1] = PointInRect(x2, y1);
            _rectVertices[2] = PointInRect(x2, y2);
            _rectVertices[3] = PointInRect(x1, y2);

            Handles.DrawSolidRectangleWithOutline(
                _rectVertices,
                fill < 0 ? Color.clear : Color.white * fill,
                line < 0 ? Color.clear : Color.white * line
            );
        }

        #endregion
    }

    // A utility class for drawing a gradient preview area.
    public class PreviewDrawer
    {
        Material _material;

        public PreviewDrawer(Shader shader)
        {
            _material = new Material(Shader.Find("Hidden/Klak/Chromatics/CosineGradient/Preview"));
            _material.hideFlags = HideFlags.DontSave;
        }

        public void Cleanup()
        {
            if (_material != null) Object.DestroyImmediate(_material);
            _material = null;
        }

        public void DrawPreview(CosineGradient grad)
        {
            _material.SetVector("_CoeffsA", grad.coeffsA);
            _material.SetVector("_CoeffsB", grad.coeffsB);
            _material.SetVector("_CoeffsC", grad.coeffsC2);
            _material.SetVector("_CoeffsD", grad.coeffsD2);

            EditorGUI.DrawPreviewTexture(
                GUILayoutUtility.GetRect(128, 32),
                EditorGUIUtility.whiteTexture, _material
            );
        }
    }
}
