using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour {

    [SerializeField] List<EdgeController> neightborhoodList;
    [SerializeField] GameObject cursorPrefab;

    [SerializeField] private bool isHost;

    private SpriteRenderer render;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void AddEdge(EdgeController edge)
    {
        neightborhoodList.Add(edge);
    }

    public bool IsConnectedTo(NodeController otherNode)
    {
        foreach(EdgeController edge in neightborhoodList)
        {
            if(edge.KnowOtherSide(this) == otherNode)
            {
                return true;
            }
        }

        return false;
    }

    public void ChangeColor(Color newColor)
    {
        render.color = newColor;
    }

    private void OnMouseDown()
    {
        if (isHost)
        {
            GameObject iCursor = Instantiate(cursorPrefab, cursorPrefab.transform.position, cursorPrefab.transform.rotation);
            iCursor.GetComponent<CursorBehavior>().SetCurrentNode(this);
        }
    }
}
