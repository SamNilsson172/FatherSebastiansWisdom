using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool walkable; //if you can walk on node
    public Vector3 worldPosition; //its position
    public int gridX; //its x index
    public int gridY; //its y index
    public int movmentPenalty; //the cost of moving over the node

    public int gCost; //the cost of moving to this node form the start
    public int hCost; //the cost of moving to the goal from this node
    public Node parent; //the node that comes before if it is placed in a path, needed for backtracking 
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movmentPenalty = _penalty;
    }

    public int fCost //the number that is compared against when checking what node to move to
    {
        get
        {
            return hCost + gCost;
        }
    }

    public int HeapIndex //its index in the heap
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) //to see which node should come first in the heap
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
