using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathFind : MonoBehaviour {
    [SerializeField] private LayerMask earth;
    private AStar _aStar = new AStar();
    private Stack<Node> path = new Stack<Node>();
    private List<Node> pathVisuals;
    private Node start, goal;

    private Camera _cam;

    private void Start() {
        _cam = Camera.main;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            start = NodeUnderMousePos();
            Debug.Log(start.position);
        }

        if (Input.GetMouseButtonDown(1)) {
            goal = NodeUnderMousePos();
            Debug.Log(goal.position);
        }

        if (start != null && goal != null) {
            path = _aStar.Path(start, goal, new AsTheCrowFlies());
            pathVisuals = new List<Node>();
            
            while (path.Count > 0) {
                pathVisuals.Add(path.Pop());
            }
            Debug.Log($"start && goal not null {pathVisuals.Count}");

            start = null;
            goal = null;
        }
    }

    private Node NodeUnderMousePos() {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, earth)) {
            Debug.Log("HIT EARTH");
            return NodeCreator.NearestNode(hitInfo.point);
        }

        return null;
    }

    private void OnDrawGizmos() {
        if (pathVisuals == null || pathVisuals.Count <= 0) return;
        
        Handles.color = Color.white;

        for (int i = 1; i < pathVisuals.Count; i++) {
            Handles.DrawLine(pathVisuals[i - 1].position, pathVisuals[i].position, .2f);
        }
    }
}
