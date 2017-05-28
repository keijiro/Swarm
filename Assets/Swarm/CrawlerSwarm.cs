// Swarm - Special renderer that draws a swarm of wobbling/crawling tubes.
// https://github.com/keijiro/Swarm

using UnityEngine;
using Klak.Chromatics;
using DFVolume;

namespace Swarm
{
    public sealed class CrawlerSwarm : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] int _instanceCount = 1000;
        [SerializeField] TubeTemplate _template;
        [SerializeField] Material _material;
        [SerializeField] CosineGradient _gradient;
        [SerializeField] VolumeData _volume;

        #endregion

        #region Hidden attributes

        [SerializeField, HideInInspector] ComputeShader _compute;

        #endregion

        #region Private fields

        ComputeBuffer _drawArgsBuffer;
        ComputeBuffer _positionBuffer;
        MaterialPropertyBlock _props;

        #endregion

        #region Compute configurations

        const int kThreadCount = 64;
        int ThreadGroupCount { get { return _instanceCount / kThreadCount; } }
        int InstanceCount { get { return kThreadCount * ThreadGroupCount; } }

        #endregion

        #region MonoBehaviour functions

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

            // Initialize the position buffer.
            _positionBuffer = new ComputeBuffer(
                (_template.segments + 1) * InstanceCount, 16
            );

            var kernel = _compute.FindKernel("CrawlerInit");
            _compute.SetInt("ArraySize", _template.segments + 1);
            _compute.SetInt("InstanceCount", InstanceCount);
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
            var kernel = _compute.FindKernel("CrawlerUpdate");
            _compute.SetFloat("Time", Time.time);
            _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
            _compute.SetTexture(kernel, "DFVolume", _volume.texture);
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

            // Draw the meshes with instancing.
            _material.SetInt("_InstanceCount", InstanceCount);
            _material.SetInt("_ArraySize", _template.segments + 1);
            _material.SetBuffer("_PositionBuffer", _positionBuffer);
            _material.SetVector("_GradientA", _gradient.coeffsA);
            _material.SetVector("_GradientB", _gradient.coeffsB);
            _material.SetVector("_GradientC", _gradient.coeffsC2);
            _material.SetVector("_GradientD", _gradient.coeffsD2);

            Graphics.DrawMeshInstancedIndirect(
                _template.mesh, 0, _material,
                new Bounds(Vector3.zero, Vector3.one * 10000),
                _drawArgsBuffer, 0, _props
            );
        }

        #endregion
    }
}
