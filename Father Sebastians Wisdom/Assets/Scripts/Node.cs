using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public bool walkable; //if player can walk through node
	public Vector3 worldPos; //the position of the node
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) //method to create nodes 
	{
		walkable = _walkable;
		worldPos = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	//public int fCost
	//{
	//	get
	//	{
	//		return gCost + fCost;
	//	}
	//}
}
