// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
// https://github.com/keijiro/Swarm

using UnityEngine;
using Klak.Chromatics;

namespace Swarm
{
    public sealed class SwirlingSwarm : MonoBehaviour
    {
        #region Basic settings

        [SerializeField] int _instanceCount = 1000;

        public int instanceCount {
            get { return _instanceCount; }
        }

        #endregion

        #region Render settings

        [SerializeField] TubeTemplate _template;

        public TubeTemplate template {
            get { return _template; }
        }

        [SerializeField] float _radius = 0.005f;

        public float radius {
            get { return _radius; }
            set { _radius = value; }
        }

        [SerializeField] Material _material;

        public Material material {
            get { return _material; }
        }

        [SerializeField] CosineGradient _gradient;

        public CosineGradient gradient {
            get { return _gradient; }
            set { _gradient = value; }
        }

        #endregion

        #region Dynamics settings

        [SerializeField] float _spread = 1;

        public float spread {
            get { return _spread; }
            set { _spread = value; }
        }

        [SerializeField] float _length = 10;

        public float length {
            get { return _length; }
            set { _length = value; }
        }

        [SerializeField] float _noiseFrequency = 4;

        public float noiseFrequency {
            get { return _noiseFrequency; }
            set { _noiseFrequency = value; }
        }

        [SerializeField] Vector3 _noiseMotion = Vector3.up * 0.2f;

        public Vector3 noiseMotion {
            get { return _noiseMotion; }
            set { _noiseMotion = value; }
        }

        #endregion

        #region Hidden attributes

        [SerializeField, HideInInspector] ComputeShader _compute;

        #endregion

        #region Private fields

        ComputeBuffer _drawArgsBuffer;
        ComputeBuffer _positionBuffer;
        ComputeBuffer _tangentBuffer;
        ComputeBuffer _normalBuffer;
        MaterialPropertyBlock _props;
        Vector3 _noiseOffset;

        #endregion

        #region Compute configurations

        const int kThreadCount = 64;
        int ThreadGroupCount { get { return _instanceCount / kThreadCount; } }
        int InstanceCount { get { return kThreadCount * ThreadGroupCount; } }
        int HistoryLength { get { return _template.segments + 1; } }

        #endregion

        #region MonoBehaviour functions

        void OnValidate()
        {
            _instanceCount = Mathf.Max(kThreadCount, _instanceCount);
        }

        void Start()
        {
            // Initialize the indirect draw args buffer.
            _drawArgsBuffer = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
            );

            _drawArgsBuffer.SetData(new uint[5] {
                _template.mesh.GetIndexCount(0),
                (uint)InstanceCount, 0, 0, 0
            });

            // Allocate compute buffers.
            _positionBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
            _tangentBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
            _normalBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);

            // Initialize the update kernel.
            var kernel = _compute.FindKernel("SwirlingUpdate");
            _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
            _compute.SetInt("InstanceCount", InstanceCount);
            _compute.SetInt("HistoryLength", HistoryLength);

            // Initialize the reconstruction kernel.
            kernel = _compute.FindKernel("SwirlingReconstruct");
            _compute.SetBuffer(kernel, "PositionBufferRO", _positionBuffer);
            _compute.SetBuffer(kernel, "TangentBuffer", _tangentBuffer);
            _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

            // Initialize the mateiral.
            _material.SetBuffer("_PositionBuffer", _positionBuffer);
            _material.SetBuffer("_TangentBuffer", _tangentBuffer);
            _material.SetBuffer("_NormalBuffer", _normalBuffer);
            _material.SetInt("_InstanceCount", InstanceCount);
            _material.SetInt("_HistoryLength", HistoryLength);

            // This property block is used only for avoiding an instancing bug.
            _props = new MaterialPropertyBlock();
            _props.SetFloat("_UniqueID", Random.value);
        }

        void OnDestroy()
        {
            _drawArgsBuffer.Release();
            _positionBuffer.Release();
            _tangentBuffer.Release();
            _normalBuffer.Release();
        }

        void Update()
        {
            _noiseOffset += _noiseMotion * Time.deltaTime;

            // Update the position buffer.
            var kernel = _compute.FindKernel("SwirlingUpdate");
            _compute.SetFloat("Spread", _spread);
            _compute.SetFloat("StepWidth", _length / _template.segments);
            _compute.SetFloat("NoiseFrequency", _noiseFrequency);
            _compute.SetVector("NoiseOffset", _noiseOffset);
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

            // Reconstruct tangent/normal vectors.
            kernel = _compute.FindKernel("SwirlingReconstruct");
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

            // Draw the meshes with instancing.
            _material.SetFloat("_Radius", _radius);
            _material.SetVector("_GradientA", _gradient.coeffsA);
            _material.SetVector("_GradientB", _gradient.coeffsB);
            _material.SetVector("_GradientC", _gradient.coeffsC2);
            _material.SetVector("_GradientD", _gradient.coeffsD2);
            _material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
            _material.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);

            Graphics.DrawMeshInstancedIndirect(
                _template.mesh, 0, _material,
                new Bounds(Vector3.zero, Vector3.one * 1.5f),
                _drawArgsBuffer, 0, _props
            );
        }

        #endregion
    }
}
