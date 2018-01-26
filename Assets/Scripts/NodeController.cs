using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour {

    [SerializeField] List<EdgeController> neightborhoodList;

    private Vector3 position;

    public void AddEdge(EdgeController edge)
    {
        neightborhoodList.Add(edge);
    }
}
