using System;
using JetBrains.Annotations;
using UnityEngine;

public class SolarSystem : MonoBehaviour {
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");
    
    private const float DayInRealTimeMinutes = 12f;
    private const float DayInRealTimeSeconds = DayInRealTimeMinutes * Seconds;
    private float _timeToInGameDay;
    private const float Seconds = 60f;
    private const float Minute = 60 * Seconds;
    private const float Hour = 60 * Minute * Seconds;
    private const float Day = Hour * 24f;
    private const float Year = 365f;
    private const float Month = Year / 12f;
    
    private float _year;
    private float _month;
    private float _day;

    private float _sunDistance;
    private Vector3 _sunPosition;
    
    [SerializeField] private Vector3 earthAxis;
    [SerializeField] private Transform sun;
    [SerializeField] private Skybox skybox;
    private Material _material;
    
    private void Awake() {
        _sunPosition = sun.position;
        _material = Instantiate(skybox.material);
        skybox.material = _material;
    }

    private void Update() {
        float day = Time.time * (1 / DayInRealTimeSeconds);
        Debug.Log(day * 360f);
        RotateSkybox(day);
        RotateSun(day);
    }

    private void RotateSkybox(float day) {
        skybox.material.SetFloat(Rotation, day * 360f);
    }

    private void RotateSun(float day) {
        Quaternion rotation = Quaternion.Euler(earthAxis * (day * 360f));
        sun.position = rotation * _sunPosition;
        
        //sun.RotateAround(Vector3.zero, earthAxis, day * 360f);
    }
    
    private void OnDestroy() {
        if (_material != null) 
            Destroy(_material);
    }
}
