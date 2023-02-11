using System;
using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    private const string MouseX = "Mouse X";
    private const string MouseY = "Mouse Y";
    
    private Vector2 _cameraRotation;
    private Vector2 _lastMousePosition;
    private Vector2 _mouseVector;
    [SerializeField] private Camera cam;
    private Vector2 _currentVelocity;
    [SerializeField] private float smoothTime = 1f;
    [SerializeField] private float maxSpeed = 10f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            _lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(1)) {
            Vector2 currentMousePos = Input.mousePosition;
            Vector2 mouseDelta = currentMousePos - _lastMousePosition;
            _mouseVector += mouseDelta;
            _mouseVector.y = Mathf.Clamp(_mouseVector.y, -89f, 89f);
            
            _lastMousePosition = currentMousePos;
        } 
        
        _lastMousePosition = Input.mousePosition;

        _cameraRotation = Vector2.SmoothDamp(
            _cameraRotation, 
            _mouseVector, 
            ref _currentVelocity, 
            smoothTime, 
            maxSpeed);
        
        transform.rotation = Quaternion.Euler(-_cameraRotation.y, _cameraRotation.x, 0);
    }
}
