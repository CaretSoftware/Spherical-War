using System;
using JetBrains.Annotations;
using UnityEngine;

public class SolarSystem : MonoBehaviour {
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");
    
    [SerializeField] private Transform sun;
    [SerializeField] private float rotation;
    [SerializeField] private Skybox skybox;
    private Material _material;
    
    private void Awake() {
        _material = Instantiate(skybox.material);
        skybox.material = _material;
    }

    private void Update() {
        rotation = Time.time * .1f % 360f;
        skybox.material.SetFloat(Rotation, rotation);
    }

    private void OnDestroy() {
        if (_material != null) 
            Destroy(_material);
    }
}
