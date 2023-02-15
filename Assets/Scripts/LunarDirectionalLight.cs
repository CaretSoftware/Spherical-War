using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LunarDirectionalLight : MonoBehaviour {
    [SerializeField] private Transform sun;
    [SerializeField] private Transform earth;
    [SerializeField] private Transform moon;
    [SerializeField] private Light directionalLight;
    [SerializeField] private float maxIntensity = .5f;
    public float intensity;
    public float dot;
    public float fullness;
    private void Update() {
        Vector3 position = moon.position;
        Vector3 sunDirection = (sun.position - position).normalized;
        //Vector3 earthDirection = (earth.position - position).normalized;

        //dot = Vector3.Dot(sunDirection, earthDirection); // obliqueness
        fullness = Vector3.Dot(-moon.forward, sunDirection) * .5f + .5f;
        fullness = Ease.EaseInCirc(fullness);
        intensity = Mathf.Lerp(0f, maxIntensity, fullness);
        directionalLight.intensity = intensity;
    }
}
