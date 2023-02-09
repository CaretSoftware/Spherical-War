using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraProjectionChanger : MonoBehaviour {
    private Camera _cam;

    private Matrix4x4 _ortho;
    private Matrix4x4 _perspective;
    private float _aspect;
    [SerializeField] private float fov = 60f;
    [SerializeField] private float near = .3f;
    [SerializeField] private float far = 1000f;
    [SerializeField] private float orthographicSize = 50f;
    [SerializeField, Range(0f, 1f)] private float zoom;
    [SerializeField] private float zoomSpeed = 5f;
    private bool _orthoOn = true;

    private void Awake() {
        _cam = GetComponent<Camera>();
    }

    void Start() {
        _aspect = (float)Screen.width / (float)Screen.height;
        _ortho = Matrix4x4.Ortho(-orthographicSize * _aspect, orthographicSize * _aspect, -orthographicSize,
            orthographicSize, near, far);
        _perspective = Matrix4x4.Perspective(fov, _aspect, near, far);
        _cam.projectionMatrix = _ortho;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _orthoOn = !_orthoOn;

            Matrix4x4 projection = _orthoOn ? _ortho : _perspective;
            
            BlendToMatrix(projection, 1f);
        }

        zoom += Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        zoom = Mathf.Clamp01(zoom);
        
        _cam.projectionMatrix = MatrixLerp(Ease.EaseOutQuint(zoom));
    }

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }
    
    public Matrix4x4 MatrixLerp(float time) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(_perspective[i], _ortho[i], time);
        return ret;
    }

    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration) {
        float startTime = Time.time;
        while (Time.time - startTime < duration) {
            _cam.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            yield return 1;
        }

        _cam.projectionMatrix = dest;
    }

    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration) {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(_cam.projectionMatrix, targetMatrix, duration));
    }
}
