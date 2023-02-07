using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class SphericalMesh : MonoBehaviour {
    [SerializeField] int faceResolution;

    [SerializeField] private float globeRadius = 10f;

    [FormerlySerializedAs("landHeight")] [SerializeField] private float landHeightMagnitude = .2f;

    [SerializeField] private Texture2D heightMap;

    [SerializeField] private GameObject[] cubeFaceGameObjects;
    
    private MeshData[] _meshData;

    private void Awake() {
        
    }

    private void Start() {
        CreateCube();
    }

    private void SpherizeCube() {
        for (int cubeSide = 0; cubeSide < 6; cubeSide++) {
            Vector3[] vertices = _meshData[cubeSide].vertices;
            int numVertices = _meshData[cubeSide].vertices.Length;

            for (int vertex = 0; vertex < numVertices; vertex++) {
                Vector3 pointOnCubeToPointOnSphere = PointOnCubeToPointOnSphere(vertices[vertex]);
                Coordinate coordinate = PointToCoordinate(pointOnCubeToPointOnSphere);
                //Vector3 coordinateToPoint = CoordinateToPoint(coordinate);
                //Vector3 spherizedPointWithHeight = SpherizeWithHeight(coordinate, height);


                vertices[vertex] = SpherizeWithHeight(pointOnCubeToPointOnSphere, coordinate);
                    //pointOnCubeToPointOnSphere + (globeRadius + height * .2f);
                //vertices[vertex] = PointOnCubeToPointOnSphere(vertices[vertex]);
            }
        }
    }
    
    private Vector3 SpherizeWithHeight(Vector3 pointOnCubeToPointOnSphere, Coordinate coordinate) {
        //Vector3 coordinateToPoint = CoordinateToPoint(coordinate);

        float height = SampleHeightTexture(coordinate);

        Vector3 pointWithHeight = pointOnCubeToPointOnSphere * (globeRadius + height * landHeightMagnitude);
        return pointWithHeight;
    }
    
    private float SampleHeightTexture(Coordinate coordinate) {
        //int x = Mathf.FloorToInt(Mathf.Abs(coordinate.longitude) * heightMap.width);
        //int y = Mathf.FloorToInt(Mathf.Abs(coordinate.latitude) * heightMap.height);
        
        float normX = Mathf.InverseLerp(-Mathf.PI, Mathf.PI, coordinate.longitude); 
        float normY = Mathf.InverseLerp(-Mathf.PI * .5f, Mathf.PI * .5f, coordinate.latitude);

        int x = Mathf.FloorToInt(normX * heightMap.width);
        int y = Mathf.FloorToInt(normY * heightMap.height);
    
        float height = heightMap.GetPixel(x, y).grayscale;
        
        return height;
    }

    // Unit circle's circumference is 2PI radians

    // Calculate latitude and longitude (in radians) from point on unit sphere
    private Coordinate PointToCoordinate(Vector3 pointOnUnitSphere) {
        float latitude = Mathf.Asin(pointOnUnitSphere.y);
        float longitude = Mathf.Atan2(pointOnUnitSphere.x, -pointOnUnitSphere.z);
        return new Coordinate(latitude, longitude);
    }

    // Calculate point on unit sphere given latitude and longitude (in radians)
    private Vector3 CoordinateToPoint(Coordinate coordinate) {
        float y = Mathf.Sin(coordinate.latitude);
        float r = Mathf.Cos(coordinate.latitude);
        float x = Mathf.Sin(coordinate.longitude) * r;
        float z = -Mathf.Cos(coordinate.longitude) * r;
        return new Vector3(x, y, z);
    }

    private void CreateCube() {
        _meshData = GenerateFaces(faceResolution);
        
        SpherizeCube();
        
        int numCubeSides = _meshData.Length;
        
        for (int i = 0; i < numCubeSides; i++) {
            Mesh mesh = cubeFaceGameObjects[i].GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = _meshData[i].vertices;
            mesh.triangles = _meshData[i].triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateUVDistributionMetrics();
            mesh.uv = _meshData[i].uvs;
            // uv's
        }
    }

    private static MeshData[] GenerateFaces(int resolution) {
        MeshData[] allMeshData = new MeshData[6];
        Vector3[] faceNormals = {
            Vector3.up, 
            Vector3.down, 
            Vector3.left, 
            Vector3.right,
            Vector3.forward, 
            Vector3.back
        };

        int len = faceNormals.Length;
        for (int i = 0; i < len; i++) {
            allMeshData[i] = CreateFace(faceNormals[i], resolution);
        }

        return allMeshData;
    }

    private static MeshData CreateFace(Vector3 normal, int resolution) {
        Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
        Vector3 axisB  = Vector3.Cross(normal, axisA);
        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {

                int vertexIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1f);
                Vector3 point = normal + axisA * (2 * t.x - 1) + axisB * (2 * t.y - 1);
                vertices[vertexIndex] = point;
                uvs[vertexIndex] = new Vector2((float)x / resolution, (float)y / resolution);

                if (x != resolution - 1 && y != resolution - 1) {
                    triangles[triIndex + 0] = vertexIndex;
                    triangles[triIndex + 1] = vertexIndex + resolution + 1;
                    triangles[triIndex + 2] = vertexIndex + resolution;
                    triangles[triIndex + 3] = vertexIndex;
                    triangles[triIndex + 4] = vertexIndex + 1;
                    triangles[triIndex + 5] = vertexIndex + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        return new MeshData(vertices, triangles, uvs);
    }

    //public static Vector3 PointOnCubeToPointOnSphere(Vector3 p) => Vector3.Normalize(p);
    private static Vector3 PointOnCubeToPointOnSphere(Vector3 p) {
        float x2 = p.x * p.x;
        float y2 = p.y * p.y;
        float z2 = p.z * p.z;

        float x = p.x * Mathf.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
        float y = p.y * Mathf.Sqrt(1 - (z2 + x2) / 2 + (z2 * x2) / 3);
        float z = p.z * Mathf.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);
        return new Vector3(x, y, z);
    }
}

public struct Coordinate {
    public Coordinate(float latitude, float longitude) {
        this.longitude = longitude;
        this.latitude = latitude;
    }

    public float longitude;
    public float latitude;
}

public struct MeshData {
    public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs) {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
} 