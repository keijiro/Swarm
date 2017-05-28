using UnityEngine;

namespace Klak.Chromatics
{
    /// Cosine gradient object
    [CreateAssetMenu(order = 1000)]
    public class CosineGradient : ScriptableObject
    {
        #region Serialized fields

        [SerializeField] Vector4 _redCoeffs   = new Vector4(0.5f, 0.5f, 1, 0);
        [SerializeField] Vector4 _greenCoeffs = new Vector4(0.5f, 0.5f, 1, 0.333f);
        [SerializeField] Vector4 _blueCoeffs  = new Vector4(0.5f, 0.5f, 1, 0.665f);

        #endregion

        #region Public accessors

        /// Packed coefficients of the red component (A, B, C, D).
        public Vector4 redCoeffs {
            get { return _redCoeffs; }
            set { _redCoeffs = value; }
        }

        /// Packed coefficients of the green component (A, B, C, D).
        public Vector4 greenCoeffs {
            get { return _greenCoeffs; }
            set { _greenCoeffs = value; }
        }

        /// Packed coefficients of the blue component (A, B, C, D).
        public Vector4 blueCoeffs {
            get { return _blueCoeffs; }
            set { _blueCoeffs = value; }
        }

        #endregion

        #region Public accessors (read only)

        /// A-coefficient of the gradient packed in (red, green, blue).
        public Vector3 coeffsA {
            get { return new Vector3(_redCoeffs.x, _greenCoeffs.x, _blueCoeffs.x); }
        }

        /// B-coefficient of the gradient packed in (red, green, blue).
        public Vector3 coeffsB {
            get { return new Vector3(_redCoeffs.y, _greenCoeffs.y, _blueCoeffs.y); }
        }

        /// C-coefficient of the gradient packed in (red, green, blue).
        public Vector3 coeffsC {
            get { return new Vector3(_redCoeffs.z, _greenCoeffs.z, _blueCoeffs.z); }
        }

        /// C-coefficients multiplied by PI * 2.
        public Vector3 coeffsC2 {
            get { return coeffsC * Mathf.PI * 2; }
        }

        /// D-coefficient of the gradient packed in (red, green, blue).
        public Vector3 coeffsD {
            get { return new Vector3(_redCoeffs.w, _greenCoeffs.w, _blueCoeffs.w); }
        }

        /// D-coefficients multiplied by PI * 2.
        public Vector3 coeffsD2 {
            get { return coeffsD * Mathf.PI * 2; }
        }

        #endregion

        #region Public methods

        /// Evaluate a color at a given point in the gradient.
        public Color Evaluate(float t)
        {
            var r = Mathf.Cos((  _redCoeffs.z * t +   _redCoeffs.w) * Mathf.PI * 2);
            var g = Mathf.Cos((_greenCoeffs.z * t + _greenCoeffs.w) * Mathf.PI * 2);
            var b = Mathf.Cos(( _blueCoeffs.z * t +  _blueCoeffs.w) * Mathf.PI * 2);
            r = Mathf.Clamp01(  _redCoeffs.x +   _redCoeffs.y * r);
            g = Mathf.Clamp01(_greenCoeffs.x + _greenCoeffs.y * g);
            b = Mathf.Clamp01( _blueCoeffs.x +  _blueCoeffs.y * b);
            return new Color(r, g, b);
        }

        #endregion
    }
}
