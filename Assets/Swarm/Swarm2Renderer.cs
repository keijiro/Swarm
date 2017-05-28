/*
using UnityEngine;
using Klak.Chromatics;

namespace Swarm
{
    public sealed class Swarm2Renderer : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] Tube _tube;
        [SerializeField] Material _material;
        [SerializeField] int _instanceCount = 1000;
        [SerializeField] CosineGradient _gradient;

        #endregion

        #region Hidden attributes

        [SerializeField, HideInInspector] ComputeShader _compute;

        #endregion

        #region Private fields

        uint[] _drawArgs = new uint[5] { 0, 0, 0, 0, 0 };
        ComputeBuffer _drawArgsBuffer;

        ComputeBuffer _positionBuffer;

        Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 128*64);
        MaterialPropertyBlock _props;

        #endregion

        #region Compute configurations

        const int kThreadCount = 64;

        int ThreadGroupCount {
            get { return _instanceCount / kThreadCount; }
        }

        int InstanceCount {
            get { return kThreadCount * ThreadGroupCount; }
        }

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            // Initialize the draw args buffer.
            _drawArgsBuffer = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
            );

            _drawArgs[0] = _tube.mesh.GetIndexCount(0);
            _drawArgs[1] = (uint)InstanceCount;
            _drawArgsBuffer.SetData(_drawArgs);

            // Initialize the position buffer.
            _positionBuffer = new ComputeBuffer(
                (_tube.segments + 1) * InstanceCount, 16
            );

            var kernel = _compute.FindKernel("SwarmUpdate");
            _compute.SetInt("ArraySize", _tube.segments + 1);
            _compute.SetInt("InstanceCount", InstanceCount);
            _compute.SetFloat("Time", Time.time);
            _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

            // This property block is used only for avoiding an instancing bug.
            _props = new MaterialPropertyBlock();
            _props.SetFloat("_UniqueID", Random.value);
        }

        void OnDestroy()
        {
            _drawArgsBuffer.Release();
            _positionBuffer.Release();
        }

        void Update()
        {
            // Update the position buffer.
            var kernel = _compute.FindKernel("SwarmUpdate");
            _compute.SetFloat("Time", Time.time);
            _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

            // Draw the mesh with instancing.
            _material.SetInt("_ArraySize", _tube.segments + 1);
            _material.SetBuffer("_PositionBuffer", _positionBuffer);
            _material.SetInt("_InstanceCount", InstanceCount);
            _material.SetVector("_GradientA", _gradient.coeffsA);
            _material.SetVector("_GradientB", _gradient.coeffsB);
            _material.SetVector("_GradientC", _gradient.coeffsC2);
            _material.SetVector("_GradientD", _gradient.coeffsD2);
            Graphics.DrawMeshInstancedIndirect(
                _tube.mesh, 0, _material, _bounds,
                _drawArgsBuffer, 0, _props
            );
        }

        #endregion
    }
}
*/
