// DFVolume - Distance field volume generator for Unity
// https://github.com/keijiro/DFVolume

using UnityEngine;

namespace DFVolume
{
    [ExecuteInEditMode]
    public class VolumeVisualizer : MonoBehaviour
    {
        enum Mode { Distance, Gradient }

        [SerializeField] VolumeData _data;
        [SerializeField] Mode _mode;
        [SerializeField, Range(0, 1)] float _depth = 0.5f;

        [SerializeField, HideInInspector] Mesh _quadMesh;
        [SerializeField, HideInInspector] Shader _shader;

        Material _material;

        void OnDestroy()
        {
            if (_material != null)
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
        }

        void Update()
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetTexture("_MainTex", _data.texture);
            _material.SetFloat("_Depth", _depth);
            _material.SetFloat("_Mode", (int)_mode);

            Graphics.DrawMesh(
                _quadMesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }
    }
}
