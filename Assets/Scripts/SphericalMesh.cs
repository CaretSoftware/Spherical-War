using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class SphericalMesh : MonoBehaviour {
    private const int NumCubeFaces = 6;
    
    [SerializeField] private int earthFaceResolution = 512;
    [SerializeField] private int waterFaceResolution = 256;
    [SerializeField] private int moonFaceResolution = 64;

    [SerializeField] private float globeRadius = 10f;
    [SerializeField] private float oceanRadius = 10f;
    [SerializeField] private float moonRadius = 5f;

    [SerializeField] private float landHeightMagnitude = .2f;

    [FormerlySerializedAs("heightMap")] [SerializeField] private Texture2D heightMapEarth;

    [SerializeField] private GameObject[] earthCubeFaces;
    [SerializeField] private GameObject[] waterCubeFaces;
    [SerializeField] private GameObject[] moonCubeFaces;
    
    private MeshData[] _meshData;

    private void Start() {
        CreateCube(earthCubeFaces, earthFaceResolution, globeRadius, heightMapEarth);
        CreateCube(waterCubeFaces, waterFaceResolution, oceanRadius);
        CreateCube(moonCubeFaces, moonFaceResolution, moonRadius);
    }

    private void CreateCube(GameObject[] cubeFaces, int resolution, float radius, Texture2D heightMap = null) {
        _meshData = GenerateFaces(resolution);
        
        SpherizeCube(heightMap, radius);
        
        for (int i = 0; i < NumCubeFaces; i++) {
            Mesh mesh = cubeFaces[i].GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = _meshData[i].vertices;
            mesh.triangles = _meshData[i].triangles;
            mesh.uv = _meshData[i].uvs;
            //mesh.normals[i] =  ;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateUVDistributionMetrics();
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
        Vector2[] uvs = null;//new Vector2[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {

                int vertexIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1f);
                Vector3 point = normal + axisA * (2 * t.x - 1) + axisB * (2 * t.y - 1);
                vertices[vertexIndex] = point;
                //uvs[vertexIndex] = new Vector2((float)x / resolution, (float)y / resolution);

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
    
    private void SpherizeCube(Texture2D heightMap, float radius) {
        for (int cubeSide = 0; cubeSide < NumCubeFaces; cubeSide++) {
            Vector3[] vertices = _meshData[cubeSide].vertices;
            int numVertices = _meshData[cubeSide].vertices.Length;
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int vertex = 0; vertex < numVertices; vertex++) {
                Vector3 pointOnSphere = SphereCoordinates.PointOnCubeToPointOnSphere(vertices[vertex]);
                CoordinatePoint coordinatePoint = SphereCoordinates.PointToCoordinate(pointOnSphere);
                //Vector3 coordinateToPoint = CoordinateToPoint(coordinatePoint);
                //Vector3 spherizedPointWithHeight = SpherizeWithHeight(coordinatePoint, height);

                uvs[vertex] = SphereCoordinates.UVCoord(coordinatePoint);
                vertices[vertex] = SpherizeWithHeight(pointOnSphere, coordinatePoint, radius, heightMap);
                    //pointOnCubeToPointOnSphere + (globeRadius + height * .2f);
                //vertices[vertex] = PointOnCubeToPointOnSphere(vertices[vertex]);
            }

            _meshData[cubeSide].uvs = uvs;
        }
    }
    
    private Vector3 SpherizeWithHeight(Vector3 pointOnCubeToPointOnSphere, CoordinatePoint coordinatePoint, float radius, Texture2D heightTexture) {
        //Vector3 coordinateToPoint = CoordinateToPoint(coordinatePoint);
        float height = heightTexture == null ? 0f : SampleHeightTexture(coordinatePoint);
        
        Vector3 pointWithHeight = pointOnCubeToPointOnSphere * (radius + height * landHeightMagnitude);
        return pointWithHeight;
    }
    
    private float SampleHeightTexture(CoordinatePoint coordinatePoint) {
        
        // TODO fix seam at edge of texture
        // TODO fix precision issue
        
        Vector2 uvCoord = SphereCoordinates.UVCoord(coordinatePoint);

        int x = Mathf.RoundToInt(uvCoord.x * heightMapEarth.width);
        int y = Mathf.RoundToInt(uvCoord.y * heightMapEarth.height);

        float height = heightMapEarth.GetPixel(x, y).grayscale; // TODO get average of surrounding pixels?
        
        return height;
    }
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
