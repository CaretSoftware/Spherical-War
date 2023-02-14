using System;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float zoomDampTime = .2f;
    [SerializeField] private Vector3 zoomedInPos;
    [SerializeField] private Vector3 zoomedOutPos;

    [SerializeField] private float orthographicSize = 12f;
    [SerializeField] private float fov = 80f;
    [SerializeField] private float near = .3f;
    [SerializeField] private float far = 100f;
    private float _aspect;
    private Matrix4x4 _ortho;
    private Matrix4x4 _perspective;
    
    private Transform _camTransform;
    
    private float _zoom = 1f;
    private float _targetZoom = 1f;
    private float zoomVelocity;
    
    private Vector3 currentLocalCameraPosition;
    private Vector3 cameraTargetPosition;
    private Vector3 currentVelocity;

    private void Awake() {
        _camTransform = cam.transform;
        _aspect = (float)Screen.width / (float)Screen.height;

        _ortho = Matrix4x4.Ortho(-orthographicSize * _aspect, orthographicSize * _aspect, -orthographicSize,
            orthographicSize, near, far);
        _perspective = Matrix4x4.Perspective(fov, _aspect, near, far);
    }

    private void Update() {
        _targetZoom += Input.mouseScrollDelta.y * zoomSpeed * Time.unscaledDeltaTime;
        _targetZoom = Mathf.Clamp01(_targetZoom);
        
        _zoom = Mathf.SmoothDamp(
            _zoom,
            _targetZoom,
            ref zoomVelocity,
            zoomDampTime);

        _camTransform.localPosition = Vector3.Lerp(zoomedOutPos, zoomedInPos, _zoom);
        cam.projectionMatrix = MatrixLerp(Ease.EaseOutCirc(_zoom));
    }
    
    public Matrix4x4 MatrixLerp(float zoom) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(_perspective[i], _ortho[i], zoom);
        return ret;
    }
}
