using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class NodeCreator : MonoBehaviour {

    private static Node[] _nodes;
    public int graphSize = 10000;
    public float height = 5f;

    public static Node NearestNode(Vector3 point) {
        Heap<Node> heap = HeapFromDistance(point, _nodes.Length);
        for (int i = 0; i < _nodes.Length; i++)
            heap.Insert(_nodes[i]);
        
        return heap.DeleteMin();
    }
    
    private void Start() {
        _nodes = new Node[graphSize];
        Vector3[] points = PointsAroundSphere(graphSize);
        
        //_neighbours = NeighboursFromNodes(_nodes, 130, 8);
        
        for (int i = 0; i < graphSize; i++) {
            _nodes[i] = new Node();
            _nodes[i].position = points[i];
        }
        
        for (int i = 0; i < graphSize; i++) {
            _nodes[i].neighbours = 
                NeighboursFromNodes(_nodes, i, 8);
        }
        
    }

    private Node[] NeighboursFromNodes(Node[] nodes, int point, int NumNeighbours) {
        Node[] neighbors = new Node[NumNeighbours];
        
        Heap<Node> heap = HeapFromDistance(nodes[point].position, nodes.Length);
        for (int i = 0; i < nodes.Length; i++) {
            if (i == point) continue;
            heap.Insert(nodes[i]);
        }

        for (int i = 0; i < NumNeighbours; i++) {
            neighbors[i] = heap.DeleteMin();
        }

        return neighbors;
    }

    // Fibonacci Sphere
    private Vector3[] PointsAroundSphere(int numPoints) {
        //def fibonacci_sphere(samples=1000):
        
        // _nodes = []
        Vector3[] points = new Vector3[numPoints];
        // phi = math.pi * (3. - math.sqrt(5.))  # golden angle in radians
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f)); // golden angle in radians;

        // for i in range(samples):
        for (int i = 0; i < numPoints; ++i) {
            
            // y = 1 - (i / float(samples - 1)) * 2  # y goes from 1 to -1
            float y = 1f - ((float)i / (numPoints - 1)) * 2f; // y goes from 1 to -1
            // radius = math.sqrt(1 - y * y)  # radius at y
            float radius = Mathf.Sqrt(1f - y * y);

            // theta = phi * i  # golden angle increment
            float theta = phi * i; // golden angle increment

            // x = math.cos(theta) * radius
            float x = Mathf.Cos(theta) * radius;
            
            // z = math.sin(theta) * radius
            float z = Mathf.Sin(theta) * radius;
            
            // _nodes.append((x, y, z))
            points[i] = new Vector3(x, y, z) * height;
            //Debug.Log($"theta: {theta} radius: {radius}");
        }

        return points;
    }

    private static Heap<Node> HeapFromDistance(Vector3 pos, int capacity) {
        return new Heap<Node>(Comparer<Node>.Create((x, y) => 
                Vector3.Distance(x.position, pos).CompareTo(Vector3.Distance(y.position, pos))), 
            capacity);
    }
    
    private static Heap<Vector3> HeapFormSqrDistance(Vector3 pos, int capacity) {
        return new Heap<Vector3>(Comparer<Vector3>.Create((x, y) =>
                (x - pos).sqrMagnitude.CompareTo((y - pos).sqrMagnitude)), 
            capacity);
    }

    public Transform cam;
    private Vector3[] _neighbours;
    private void OnDrawGizmosSelected() {
        if (_nodes == null || _nodes.Length == 0) return;

        Handles.zTest = CompareFunction.GreaterEqual;
        Gizmos.color = Color.red;
        Handles.color = Color.cyan;
        Vector3 fwd = cam.forward;
        
        for (int i = 0; i < _nodes.Length; i++) {
            Vector3 pos = _nodes[i].position;
            if (Vector3.Dot(pos.normalized, fwd) > 0f)
                continue;
            Gizmos.DrawSphere(pos, .01f);
            for (int j = 0; j < _nodes[i].neighbours.Length; j++) {
                Handles.DrawLine(_nodes[i].position, _nodes[i].neighbours[j].position);
            }
        }
    }
}
