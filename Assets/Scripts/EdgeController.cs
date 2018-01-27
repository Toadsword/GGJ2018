using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour {

    [SerializeField] NodeController nodeEdge1;
    [SerializeField] NodeController nodeEdge2;

    private GameManager gameManager;

    private SpriteRenderer render;

    public int idMessage{get;private set;} // -1 for no connection on the node

	// Use this for initialization
	void Start () {
        idMessage = -1;
        nodeEdge1.AddEdge(this);
        nodeEdge2.AddEdge(this);

        render = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        if(idMessage>=0)
            ChangeColor(gameManager.colorArray[idMessage%10]);
        else
            ChangeColor(Color.black);
        return result;
    }

    public void ResetPath()
    {
        idMessage = -1;
    }

    public void ChangeColor(Color newColor) {
        render.color = newColor;
    }

}
