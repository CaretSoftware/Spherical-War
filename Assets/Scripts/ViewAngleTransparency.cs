using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ViewAngleTransparency : MonoBehaviour {
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    
    [SerializeField] private Transform cam;
    [SerializeField] private Transform atmosphereShadow;
    [FormerlySerializedAs("_alphaFull")] [SerializeField] private float alphaFull;
    
    private Color color = Color.black;
    private Material _material;

    private void Awake() {
        _material = Instantiate(atmosphereShadow.GetComponent<MeshRenderer>().material);
        
    }

    private void Update() {
        Vector3 direction = (cam.position - atmosphereShadow.position).normalized;
        float dot = Vector3.Dot(direction, atmosphereShadow.forward);
        float t = Mathf.InverseLerp(-1f, 1f, dot);
        float alpha = Mathf.Lerp(0f, alphaFull, t);
        color.a = alpha;
        _material.SetColor(Color1, color);
    }
}
