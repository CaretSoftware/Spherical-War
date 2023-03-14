using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class HeapTest : MonoBehaviour {
    private Heap<Vector3> heap;
    private System.Random rand = new System.Random(1234);
    public int numTest = 1000;
    public bool success;
    public bool sqrt = false;
    Stopwatch _stopwatch = new Stopwatch();
    
    private void Start() {
        _stopwatch.Start();
        Vector3 pos = Vector3.one;

        heap = sqrt ? HeapFormDistance(pos, numTest) : HeapFormSqrDistance(pos, numTest);
        //heap = HeapFormDistance(pos, numTest);
            // new Heap<Vector3>(Comparer<Vector3>.Create((x, y) => 
            // Vector3.Distance(x, pos).CompareTo(Vector3.Distance(y, pos))), 
            // numTest);

        for (int i = 0; i < numTest; i++) {
            heap.Insert( 
                new Vector3((float)rand.NextDouble() % 10f, (float)rand.NextDouble() % 10f, (float)rand.NextDouble() % 10f)
                //rand.Next()
                );
        }

        float min = float.MinValue;

        bool working = true;
        for (int i = 0; i < numTest && !heap.Empty(); i++) {
            Vector3 next = heap.DeleteMin();
            float dist = Vector3.Distance(pos, next);
            //Debug.Log(dist);
            
            //Debug.Log(next);
            if (dist < min) {
                Debug.Log($"{dist} {min}");
                working = false;
            }
            min = dist;
        }

        success = working;
        _stopwatch.Stop();
        Debug.Log($"{working} {_stopwatch.ElapsedMilliseconds}");
    }
    
    private Heap<Vector3> HeapFormSqrDistance(Vector3 pos, int capacity) {
        return new Heap<Vector3>(Comparer<Vector3>.Create((x, y) =>
                (x - pos).sqrMagnitude.CompareTo((y - pos).sqrMagnitude)), 
            capacity);
    }
    
    private Heap<Vector3> HeapFormDistance(Vector3 pos, int capacity) {
        return new Heap<Vector3>(Comparer<Vector3>.Create((x, y) => 
                Vector3.Distance(x, pos).CompareTo(Vector3.Distance(y, pos))), 
            capacity);
    }
}
