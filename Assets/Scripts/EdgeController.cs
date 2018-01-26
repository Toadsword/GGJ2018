using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour {

    [SerializeField] NodeController nodeEdge1;
    [SerializeField] NodeController nodeEdge2;

    [SerializeField] private bool isHost;

    private int idMessage = -1; // --1 for no connection on the node

	// Use this for initialization
	void Start () {
        idMessage = -1;
        nodeEdge1.AddEdge(this);
        nodeEdge2.AddEdge(this);
    }

    public bool IsTaken()
    {
        return idMessage >= 0;
    }

    public NodeController KnowOtherSide(NodeController source)
    {
        if (source == nodeEdge1)
            return nodeEdge2;
        else if (source == nodeEdge2)
            return nodeEdge1;

        Debug.Log("Not a liaison for source" + source);
        return null;
    }

    // return if a previous path got deleted
    public bool TakePath(int newId)
    {
        bool result = IsTaken();
        idMessage = newId;
        return result;
    }

    public void ResetPath()
    {
        idMessage = -1;
    }

}
