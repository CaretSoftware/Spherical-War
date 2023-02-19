using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof(RebuildSphere))]
public class RebuildSphereEditor : Editor {
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Reset Sphere")) {
            RebuildSphere.ResetSphere(Selection.activeGameObject.GetComponent<RebuildSphere>().MF.sharedMesh);
        }
    }
}
