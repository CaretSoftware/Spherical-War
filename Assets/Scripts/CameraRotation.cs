using System;
using System.Collections;
using UnityEditor;
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
    [SerializeField] private float globeElevation = 10f;
    [SerializeField] private float mouseRotationFactor = 10f;
    private Plane _plane;
    private Ray _centerScreenRay;
    private Ray _ray;
    
    private void Update() {
        if (Input.GetMouseButtonDown(1))
            _lastMousePosition = MousePositionOnPlane();

        if (Input.GetMouseButton(1)) {
            Vector2 currentMousePos = MousePositionOnPlane();
            Vector2 mouseDelta = (currentMousePos - _lastMousePosition) * mouseRotationFactor;
            _mouseVector += mouseDelta;
            _mouseVector.y = Mathf.Clamp(_mouseVector.y, -89f, 89f);
            
            _lastMousePosition = currentMousePos;
        }

        _cameraRotation = Vector2.SmoothDamp(
            _cameraRotation, 
            _mouseVector, 
            ref _currentVelocity, 
            smoothTime, 
            maxSpeed);
        
        transform.rotation = Quaternion.Euler(-_cameraRotation.y, _cameraRotation.x, 0);
    }

    private Vector2 MousePositionOnPlane() {
        Vector2 hitPosition = _lastMousePosition;
        Vector3 cameraPosition = cam.transform.position;
        Vector3 planeNormal = cameraPosition.normalized;
        Vector3 planePoint = planeNormal * globeElevation;
        _plane = new Plane(planeNormal, planePoint);

        _ray = cam.ScreenPointToRay(Input.mousePosition);

        if (_plane.Raycast(_ray, out float rayHitDistance)) {
            _centerScreenRay = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            _plane.Raycast(_centerScreenRay, out float centerHitDistance);
            
            Vector3 hitPoint = _ray.GetPoint(rayHitDistance);
            Vector3 hitPointCenter = _centerScreenRay.GetPoint(centerHitDistance);
            Vector3 vectorFromCenter = hitPoint - hitPointCenter;
            Vector3 inverseVector = cam.transform.InverseTransformVector(vectorFromCenter);
            hitPosition = inverseVector;
        }

        return hitPosition;
    }
}
