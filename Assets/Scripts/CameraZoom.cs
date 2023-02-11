using System;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 1;
    [SerializeField] private Vector3 zoomedInPos;
    [SerializeField] private Vector3 zoomedOutPos;
    
    private Transform _camTransform;
    private float _zoom;
    [SerializeField] private float zoomDampTime;
    private Vector3 currentLocalCameraPosition;
    private Vector3 cameraTargetPosition;
    private Vector3 currentVelocity;
    
    private void Awake() {
        _camTransform = cam.transform;
    }

    private void Update() {
        _zoom += Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        _zoom = Mathf.Clamp01(_zoom);

        cameraTargetPosition = Vector3.Lerp(zoomedOutPos, zoomedInPos, _zoom);
        
        Vector3 smoothVector = Vector3.SmoothDamp(
            currentLocalCameraPosition, 
            cameraTargetPosition, 
            ref currentVelocity, 
            zoomDampTime);
        
        _camTransform.localPosition = smoothVector;
        currentLocalCameraPosition = smoothVector;
    }
}
