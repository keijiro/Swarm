// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
// https://github.com/keijiro/Swarm

using UnityEngine;
using Klak.Chromatics;

namespace Swarm
{
    public sealed class FloatingSwarm : MonoBehaviour
    {
        #region Instancing properties

        [SerializeField] int _instanceCount = 1000;

        public int instanceCount {
            get { return _instanceCount; }
        }

        [SerializeField] TubeTemplate _template;

        public TubeTemplate template {
            get { return _template; }
        }

        [SerializeField] float _radius = 0.005f;

        public float radius {
            get { return _radius; }
            set { _radius = value; }
        }

        [SerializeField, Range(0, 1)] float _trim = 1;

        public float trim {
            get { return _trim; }
            set { _trim = value; }
        }

        #endregion

        #region Dynamics and attractor properteis

        [SerializeField] Transform _attractor;

        public Transform attractor {
            get { return _attractor; }
            set { _attractor = value; }
        }

        [SerializeField] Vector3 _attractorPosition = Vector3.zero;

        public Vector3 attractorPosition {
            get { return _attractorPosition; }
            set { _attractorPosition = value; }
        }

        [SerializeField] float _attractorSpread = 0.01f;

        public float attractorSpread {
            get { return _attractorSpread; }
            set { _attractorSpread = value; }
        }

        [SerializeField] float _attractorForce = 5.0f;

        public float attractorForce {
            get { return _attractorForce; }
            set { _attractorForce = value; }
        }

        [SerializeField, Range(0, 1)] float _forceRandomness = 0.5f;

        public float forceRandomness {
            get { return _forceRandomness; }
            set { _forceRandomness = value; }
        }

        [SerializeField, Range(0, 6)] float _drag = 2.0f;

        public float drag {
            get { return _drag; }
            set { _drag = value; }
        }

        #endregion

        #region Noise field properties

        [SerializeField] float _headNoiseForce = 0.5f;

        public float headNoiseForce {
            get { return _headNoiseForce; }
            set { _headNoiseForce = value; }
        }

        [SerializeField] float _headNoiseFrequency = 0.5f;

        public float headNoiseFrequency {
            get { return _headNoiseFrequency; }
            set { _headNoiseFrequency = value; }
        }

        [SerializeField] float _trailNoiseVelocity = 0.01f;

        public float trailNoiseVelocity {
            get { return _trailNoiseVelocity; }
            set { _trailNoiseVelocity = value; }
        }

        [SerializeField] float _trailNoiseFrequency = 0.5f;

        public float trailNoiseFrequency {
            get { return _trailNoiseFrequency; }
            set { _trailNoiseFrequency = value; }
        }

        [SerializeField] float _noiseSpread = 0.5f;

        public float noiseSpread {
            get { return _noiseSpread; }
            set { _noiseSpread = value; }
        }

        [SerializeField] float _noiseMotion = 0.15f;

        public float noiseMotion {
            get { return _noiseMotion; }
            set { _noiseMotion = value; }
        }

        #endregion

        #region Material properties

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

        #region Misc properties

        [SerializeField] int _randomSeed;

        public int randomSeed {
            set { _randomSeed = value; }
        }

        #endregion

        #region Hidden attributes

        [SerializeField, HideInInspector] ComputeShader _compute;

        #endregion

        #region Private members

        ComputeBuffer _drawArgsBuffer;
        ComputeBuffer _positionBuffer;
        ComputeBuffer _velocityBuffer;
        ComputeBuffer _tangentBuffer;
        ComputeBuffer _normalBuffer;
        bool _materialCloned;
        MaterialPropertyBlock _props;
        Vector3 _noiseOffset;

        Vector4 AttractorVector {
            get {
                var p = _attractor ? _attractor.position : _attractorPosition;
                p = transform.InverseTransformPoint(p);
                return new Vector4(p.x, p.y, p.z, _attractorSpread);
            }
        }

        #endregion

        #region Compute configurations

        const int kThreadCount = 64;
        int ThreadGroupCount { get { return _instanceCount / kThreadCount; } }
        int InstanceCount { get { return kThreadCount * ThreadGroupCount; } }
        int HistoryLength { get { return _template.segments + 1; } }

        #endregion

        #region Public Methods

        public void ResetPositions()
        {
            if (_positionBuffer != null)
            {
                // Invoke the initialization kernel.
                var kernel = _compute.FindKernel("FloatingInit");
                _compute.SetInt("InstanceCount", InstanceCount);
                _compute.SetInt("HistoryLength", HistoryLength);
                _compute.SetVector("Attractor", AttractorVector);
                _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
                _compute.SetBuffer(kernel, "VelocityBuffer", _velocityBuffer);
                _compute.SetBuffer(kernel, "TangentBuffer", _tangentBuffer);
                _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);
                _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);
            }
        }

        #endregion

        #region MonoBehaviour functions

        void OnValidate()
        {
            _instanceCount = Mathf.Max(kThreadCount, _instanceCount);
            _radius = Mathf.Max(0, _radius);
            _attractorSpread = Mathf.Max(0, _attractorSpread);
            _headNoiseForce = Mathf.Max(0, _headNoiseForce);
            _headNoiseFrequency = Mathf.Max(0, _headNoiseFrequency);
            _trailNoiseVelocity = Mathf.Max(0, _trailNoiseVelocity);
            _trailNoiseFrequency = Mathf.Max(0, _trailNoiseFrequency);
            _noiseSpread = Mathf.Max(0, _noiseSpread);
        }

        void Start()
        {
            // Initialize the indirect draw args buffer.
            _drawArgsBuffer = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
            );

            _drawArgsBuffer.SetData(new uint[5] {
                _template.mesh.GetIndexCount(0), (uint)InstanceCount, 0, 0, 0
            });

            // Allocate compute buffers.
            _positionBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
            _velocityBuffer = new ComputeBuffer(InstanceCount, 16);
            _tangentBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
            _normalBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);

            ResetPositions();

            // This property block is used only for avoiding an instancing bug.
            _props = new MaterialPropertyBlock();
            _props.SetFloat("_UniqueID", Random.value);

            // Clone the given material before using.
            _material = new Material(_material);
            _material.name += " (cloned)";
            _materialCloned = true;

            _noiseOffset = Vector3.one * _randomSeed;
        }

        void OnDestroy()
        {
            if (_drawArgsBuffer != null) _drawArgsBuffer.Release();
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_velocityBuffer != null) _velocityBuffer.Release();
            if (_tangentBuffer != null) _tangentBuffer.Release();
            if (_normalBuffer != null) _normalBuffer.Release();
            if (_materialCloned) Destroy(_material);
        }

        void Update()
        {
            var delta = Mathf.Min(Time.deltaTime, 1.0f / 30);

            if (delta > 0)
            {
                // Invoke the update compute kernel.
                var kernel = _compute.FindKernel("FloatingUpdate");

                _compute.SetInt("_InstanceCount", InstanceCount);
                _compute.SetInt("_HistoryLength", HistoryLength);

                _compute.SetFloat("RandomSeed", _randomSeed);
                _compute.SetFloat("DeltaTime", delta);

                _compute.SetVector("Attractor", AttractorVector);
                var minForce = _attractorForce * (1 - _forceRandomness);
                _compute.SetVector("Force", new Vector2(minForce, _attractorForce));
                _compute.SetFloat("Drag", Mathf.Exp(-_drag * delta));

                _compute.SetFloat("HeadNoiseForce", _headNoiseForce);
                _compute.SetFloat("HeadNoiseFrequency", _headNoiseFrequency);
                _compute.SetFloat("TrailNoiseVelocity", _trailNoiseVelocity);
                _compute.SetFloat("TrailNoiseFrequency", _trailNoiseFrequency);
                _compute.SetFloat("NoiseSpread", _noiseSpread / InstanceCount);
                _compute.SetVector("NoiseOffset", _noiseOffset);

                _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
                _compute.SetBuffer(kernel, "VelocityBuffer", _velocityBuffer);

                _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

                // Invoke the reconstruction kernel.
                kernel = _compute.FindKernel("FloatingReconstruct");

                _compute.SetBuffer(kernel, "PositionBufferRO", _positionBuffer);
                _compute.SetBuffer(kernel, "TangentBuffer", _tangentBuffer);
                _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

                _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);
            }

            // Draw the mesh with instancing.
            _material.SetFloat("_Radius", _radius);

            _material.SetVector("_GradientA", _gradient.coeffsA);
            _material.SetVector("_GradientB", _gradient.coeffsB);
            _material.SetVector("_GradientC", _gradient.coeffsC2);
            _material.SetVector("_GradientD", _gradient.coeffsD2);

            _material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
            _material.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);

            _material.SetBuffer("_PositionBuffer", _positionBuffer);
            _material.SetBuffer("_TangentBuffer", _tangentBuffer);
            _material.SetBuffer("_NormalBuffer", _normalBuffer);

            _material.SetInt("_InstanceCount", InstanceCount);
            _material.SetInt("_HistoryLength", HistoryLength);
            _material.SetInt("_IndexLimit", (int)(_trim * HistoryLength));

            Graphics.DrawMeshInstancedIndirect(
                _template.mesh, 0, _material,
                new Bounds(Vector3.zero, Vector3.one * 1000),
                _drawArgsBuffer, 0, _props
            );

            // Move the noise field.
            _noiseOffset += Vector3.one * _noiseMotion * delta;
        }

        #endregion
    }
}
