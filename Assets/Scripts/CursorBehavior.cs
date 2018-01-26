using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehavior : MonoBehaviour {

    [SerializeField] Color colorHost;

    private NodeController currentNode;

    public void SetCurrentNode(NodeController newNode)
    {
        currentNode = newNode;
        currentNode.ChangeColor(colorHost);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "node")
        {
            collision.GetComponent<NodeController>().IsConnectedTo(currentNode);
        }
    }
}
