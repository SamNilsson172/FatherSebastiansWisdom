using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requsetManager;
    Gridy grid;

    void Awake()
    {
        requsetManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Gridy>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        if (startNode.walkable && targetNode.walkable)
            while (openSet.Count > 0) //only breaks if target node is unreacheble
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) //if goal is reached
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) //if open neighbour to current gets a lower gcost from using currents path(patrents), current is the neighbours parent, basically starts here
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
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requsetManager.FinishedProcessingPath(waypoints, pathSuccess);
    }
    Vector3[] RetracePath(Node firstNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != firstNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(grid.NodeFromWorldPoint(firstNode.worldPosition));

        Vector3[] waypoints = SimplyfyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplyfyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 dirOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDir = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (newDir != dirOld)
                waypoints.Add(path[i].worldPosition);
            dirOld = newDir;
        }
        return waypoints.ToArray();
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
