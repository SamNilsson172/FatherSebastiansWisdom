using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	public Transform seeker, target;
	Grid grid;

	void Update()
	{
		FindPath(seeker.position, target.position);
	}

	void Awake()
	{
		grid = GetComponent<Grid>();
	}

	void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) //only breaks if target node is unreacheble
		{
			Node currentNode = openSet[0]; //only needed for first loop?
			for (int i = 1; i < openSet.Count; i++) //looks through all nodes in open set and sets the one with the lowest fCost to current
			{
				if (openSet[i].gCost + openSet[i].hCost < currentNode.gCost + currentNode.hCost || openSet[i].gCost + openSet[i].hCost == currentNode.gCost + currentNode.hCost && openSet[i].hCost < currentNode.hCost)
					currentNode = openSet[i];
			}

			openSet.Remove(currentNode); //close current
			closedSet.Add(currentNode);

			if (currentNode == targetNode) //if goal is reached
			{
				RetracePath(startNode, targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbors(currentNode)) //if open neighbour to current gets a lower gcost from using currents path(patrents), current is the neighbours parent, basically starts here
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
					continue;

				int movenCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
				if (movenCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = movenCostToNeighbour;
					neighbour.hCost = getDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

		void RetracePath(Node firstNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;
			while (currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.parent;
			}

			path.Add(grid.NodeFromWorldPoint(startPos));
			path.Reverse();
			grid.path = path;
		}

		int getDistance(Node nodeA, Node nodeB)
		{
			int x = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int y = Mathf.Abs(nodeA.gridY - nodeB.gridY);

			if (x > y)
				return (x - y) * 10 + y * 14;
			return (y - x) * 10 + x * 14;
		}
	}
}
