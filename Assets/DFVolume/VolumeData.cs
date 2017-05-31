// DFVolume - Distance field volume generator for Unity
// https://github.com/keijiro/DFVolume

using UnityEngine;

namespace DFVolume
{
    public class VolumeData : ScriptableObject
    {
        #region Exposed attributes

        [SerializeField] Texture3D _texture;

        public Texture3D texture {
            get { return _texture; }
        }

        #endregion

        #if UNITY_EDITOR

        #region Editor functions

        public void Initialize(VolumeSampler sampler)
        {
            var dim = sampler.resolution;
            _texture = new Texture3D(dim, dim, dim, TextureFormat.RGBAHalf, true);

            _texture.name = "Distance Field Texture";
            _texture.filterMode = FilterMode.Bilinear;
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.SetPixels(sampler.GenerateBitmap());
            _texture.Apply();
        }

        #endregion

        #endif
    }
}
