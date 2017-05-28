// Swarm - Special renderer that draws a swarm of wobbling/crawling tubes.
// https://github.com/keijiro/Swarm

using UnityEngine;
using System.Collections.Generic;

namespace Swarm
{
    // Pre-built template mesh for tube shapes.
    public sealed class TubeTemplate : ScriptableObject
    {
        #region Exposed properties

        [Tooltip("Number of vertices on a ring.")]
        [SerializeField] int _divisions = 6;

        public int divisions {
            get { return Mathf.Clamp(_divisions, 2, 64); }
        }

        [Tooltip("Number of segments in a tube.")]
        [SerializeField] int _segments = 256;

        public int segments {
            get { return Mathf.Clamp(_segments, 4, 4096); }
        }

        #endregion

        #region Serialized data

        [SerializeField] Mesh _mesh;

        public Mesh mesh { get { return _mesh; } }

        #endregion

        #region Editor functions

        #if UNITY_EDITOR

        public void Rebuild()
        {
            // Vertex array
            var vertices = new List<Vector3>();

            for (var i = 0; i < _segments + 1; i++)
            {
                for (var j = 0; j < _divisions + 1; j++)
                {
                    var phi = Mathf.PI * 2 * j / _divisions;
                    vertices.Add(new Vector3(phi, 0, i));
                }
            }

            // Index array
            var indices = new List<int>();
            var refi = 0;

            for (var i = 0; i < _segments; i++)
            {
                for (var j = 0; j < _divisions; j++)
                {
                    indices.Add(refi);
                    indices.Add(refi + 1);
                    indices.Add(refi + 1 + _divisions);

                    indices.Add(refi + 1);
                    indices.Add(refi + 2 + _divisions);
                    indices.Add(refi + 1 + _divisions);

                    refi++;
                }
                refi++;
            }

            // Mesh rebuilding
            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            _mesh.UploadMeshData(true);
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "Tube Template";
            }
        }

        void OnValidate()
        {
            _divisions = Mathf.Clamp(_divisions, 2, 64);
            _segments = Mathf.Clamp(_segments, 4, 4096);
        }

        #endregion
    }
}
