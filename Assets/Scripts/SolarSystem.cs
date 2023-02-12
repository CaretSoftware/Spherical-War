using System;
using JetBrains.Annotations;
using UnityEngine;

public class SolarSystem : MonoBehaviour {
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    private const float RevolutionsToDegrees = 360f;
    private const float RealTimeMinutesPerDay = 12f;
    private const float SecondsPerDay = RealTimeMinutesPerDay * SecondsInAMinute;
    private const float SecondsPerMonth = DaysPerMonth * RealTimeMinutesPerDay * SecondsInAMinute;
    private const float SecondsPerYear = DaysPerYear * SecondsPerDay;
    
    private const float SecondsInAMinute = 60f;
    private const float DaysPerYear = 365f;
    private const float DaysPerMonth = DaysPerYear / 12f;
    
    private float _year;
    private float _month;
    private float _day;

    private float _sunDistance;
    private Vector3 _sunPosition;
    private Vector3 _moonPosition;
    
    [SerializeField] private Vector3 earthAxis;
    [SerializeField] private Vector3 moonAxis;
    [SerializeField] private Transform sun;
    [SerializeField] private Transform moon;
    [SerializeField] private Skybox skybox;
    private Material _material;
    
    private void Awake() {
        _sunPosition = sun.position;
        _moonPosition = moon.position;
        _material = Instantiate(skybox.material);
        skybox.material = _material;
    }

    private void Update() {
        DayRotation();
        MonthRotation();
    }

    private void DayRotation() {
        float day = Time.time * (1f / SecondsPerDay);
        RotateSkybox(day);
        RotateSun(day);
    }

    private void MonthRotation() {
        float month = Time.time * (1f / SecondsPerMonth);
        
        RotateMoon(month);
    }
    
    private void RotateSkybox(float day) {
        
        skybox.material.SetFloat(Rotation, -day * RevolutionsToDegrees);
    }

    private void RotateSun(float day) {
        Quaternion rotation = Quaternion.Euler(earthAxis * (day * RevolutionsToDegrees));
        sun.position = rotation * _sunPosition;
    }
    
    private void RotateMoon(float month) {
        Quaternion rotation = Quaternion.Euler(moonAxis * (month * RevolutionsToDegrees));
        moon.position = rotation * _moonPosition;
    }
    
    private void OnDestroy() {
        if (_material != null) 
            Destroy(_material);
    }
}
