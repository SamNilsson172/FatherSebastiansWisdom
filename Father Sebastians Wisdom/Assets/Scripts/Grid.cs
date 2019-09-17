using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public LayerMask obstacleMask;
	Node[,] grid; //array of all nodes

	float nodeDiameter;
	int gridSizeX, gridSizeY; //amount of nodes in the x and y axis

	void Start()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //amount of nodes in the axis x that fit in the grid
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid();
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY]; //set the grid size to the x and y values that fit
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //get the bottom left by subtracting the right most and up most value

		for (int x = 0; x < gridSizeX; x++) //loop in loop to have values for all nodes
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); //get the position of the node by going right and up and adding a radius to get it in the center
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask)); //check if the node is in an obstacle which defines if it's walkable
				grid[x, y] = new Node(walkable, worldPoint, x, y); //define node in grid with accuired values
			}
	}

	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neighbors = new List<Node>();

		for (int x = -1; x <= 1; x++)
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
					neighbors.Add(grid[checkX, node.gridY + y]);
			}
		return neighbors;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) //method to get a specific node from a specific vector
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; //get the position of the specific vector in percent by adding half the grid size (because 0,0 is in the middle of the grid) then devide by the entire grid size
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX); //if value is bigger than 1 or smaller than 0, set it to 1 or 0 
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); //get the exact position by multiplying the amount of nodes in the axis (-1 cus it starts at 0) with the percent of the current position
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x, y];
	}

	public List<Node> path;
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = n.walkable ? Color.white : Color.red;
				if (path != null)
					if (path.Contains(n))
						Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}
}
