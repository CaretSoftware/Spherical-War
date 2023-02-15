using System;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [SerializeField] private Vector3 sunAxis;
    [SerializeField] private Vector3 moonAxis;
    [SerializeField] private Transform sun;
    [SerializeField] private Transform moon;
    [SerializeField] private Skybox skybox;
    private Material _material;
    
    // Inclinations to earths Equatorial Plane, not the Ecliptic Plane
    private readonly Quaternion _sunInclination = Quaternion.Euler(-23.44f, 0f, 0f);
    private readonly Quaternion _lunarInclination = Quaternion.Euler(-23.44f - 6.68f, 0f, 0f);

    private Quaternion _seasonalSunInclination;
    private Quaternion _seasonalLunarInclination;
    private Quaternion _yearRotation;

    private Quaternion _seasonCompensation;
    private float time;
    [Range(0f, 10000f)] public float timeScale;

    private void Awake() {
        _sunPosition = sun.position;
        _moonPosition = moon.position;
        _material = Instantiate(skybox.material);
        skybox.material = _material;
    }

    private void Update() {
        time += Time.deltaTime * timeScale;
        CelestialRotations();
    }

    private void CelestialRotations() {
        float days = time * (1f / SecondsPerDay);
        float months = time * (1f / SecondsPerMonth);
        float years = time * (1f / SecondsPerYear);
        
        //Debug.Log($"d:<B>{days:N1}</B> m:<B>{months:N1}</B> y:<B>{years:N1}</B>");
        
        RotateEcliptic(years);
        RotateSun(days);
        RotateMoon(days, months);
        RotateSkybox(days);
    }

    private void RotateEcliptic(float years) {
        _yearRotation = Quaternion.Euler(years * RevolutionsToDegrees * Vector3.up);
        
        _seasonCompensation = Quaternion.Inverse(_yearRotation);
        _seasonalSunInclination = _yearRotation * _sunInclination;
        _seasonalLunarInclination = _yearRotation * _lunarInclination;
    }
    
    private void RotateSun(float days) {
        Quaternion dayRotation = Quaternion.Euler(days * RevolutionsToDegrees * sunAxis);
        Quaternion rotation = 
            _seasonalSunInclination *
            dayRotation *
            _seasonCompensation;
        
        sun.position = rotation * _sunPosition;
    }

    private void RotateMoon(float days, float months) {
        Quaternion dayRotation = Quaternion.Euler(days * RevolutionsToDegrees * moonAxis);
        Quaternion monthRotation = Quaternion.Euler(months * RevolutionsToDegrees * moonAxis);

        Quaternion rotation =
            _seasonalLunarInclination *
            dayRotation *
            monthRotation *
            _seasonCompensation;
        
        moon.position = rotation * _moonPosition;
    }

    private void RotateSkybox(float day) {
        skybox.material.SetFloat(Rotation, -day * RevolutionsToDegrees);
    }

    private void OnDestroy() {
        if (_material != null) 
            Destroy(_material);
    }
}
