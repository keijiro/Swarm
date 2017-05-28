// DFVolume - Distance field volume generator for Unity
// https://github.com/keijiro/DFVolume

using UnityEngine;
using UnityEditor;

namespace DFVolume
{
    [CustomEditor(typeof(VolumeData))]
    class VolumeDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // There is nothing to show in the inspector.
        }
    }
}
