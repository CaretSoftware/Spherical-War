using System;
using System.Collections.Generic;
using UnityEngine;

public class Node {//, IComparable<Node>, IComparable {
    public Node[] neighbours;
    public Vector3 position;
    //public float fScore = float.MaxValue;
    

    private void DrawDebugRay() {
        for (int node = 0; node < neighbours.Length; node++) {
            for (int neighbour = 0; neighbour < neighbours.Length; neighbour++) {
                Debug.DrawLine(position, neighbours[neighbour].position, Color.red);
            }
        }
    }

    public float Cost(Node other) {
        return Vector3.Distance(position, other.position);
    }

    //public int CompareTo(object obj) {
    //    Node other = obj as Node; // avoid double casting
    //    if (other == null) {
    //        throw new ArgumentException("A Node object is required for comparison.", nameof(obj));
    //    }
    //    
    //    return CompareTo(other);
    //}

    //public int CompareTo(Node obj) {
    //    return obj.fScore.CompareTo(this.fScore);
    //}

    private void OnDrawGizmosSelected() {
        if (Application.isPlaying && neighbours != null)
            DrawDebugRay();
    }

    // public override int GetHashCode() {
    //     return base.GetHashCode();
    // }
}
