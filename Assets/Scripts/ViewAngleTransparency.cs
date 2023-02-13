using System;
using UnityEngine;

public class ViewAngleTransparency : MonoBehaviour {
    private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
    
    [SerializeField] private Transform cam;
    [SerializeField] private Transform atmosphereShadowTransform;
    [SerializeField] private MeshRenderer atmosphereShadow;
    
    public Color color = Color.black;
    private Material _material;
    private float alphaMax;

    private void Awake() {
        _material = Instantiate(atmosphereShadow.material);
        atmosphereShadow.material = _material;
        color = _material.GetColor(Color1);
        alphaMax = color.a;
    }

    private void Update() {
        Vector3 direction = (cam.position - atmosphereShadowTransform.position).normalized;
        float dot = Vector3.Dot(direction, atmosphereShadowTransform.forward);
        float t = Mathf.InverseLerp(-1f, 1f, dot);
        float e = Mathf.Lerp(
            0f, 
            Mathf.Lerp(
                alphaMax * 4f, 
                0f, 
                t),
            Ease.EaseOutExpo(t));
        float alpha = Mathf.Lerp(0f, alphaMax, Ease.EaseOutCirc(e));
        color.a = alpha;
        _material.SetColor(Color1, color);
    }
}
