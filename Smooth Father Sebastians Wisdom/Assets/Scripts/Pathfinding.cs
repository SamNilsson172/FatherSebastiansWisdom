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

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) //use courotine to wait for end of frame
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch(); //check how long the method takes
        sw.Start();

        Vector3[] waypoints = new Vector3[0]; //contains the path
        bool pathSuccess = false; //if path could be found

        Node startNode = grid.NodeFromWorldPoint(startPos); //get start and end in node
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //create heap for all nodes that can be moved to
        HashSet<Node> closedSet = new HashSet<Node>(); //hash set for all nodes that can't be moved to
        openSet.Add(startNode); //add the startnode for a starting point

        if (startNode.walkable && targetNode.walkable)
            while (openSet.Count > 0) //only breaks if target node is unreacheble
            {
                Node currentNode = openSet.RemoveFirst(); //set current node to the first one in openSet and remove it from the openset 
                closedSet.Add(currentNode); //so you can't move to current node again

                if (currentNode == targetNode) //if goal is reached
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) //if an open neighbour to the current node gets a lower gcost from using currents path (patrents), current is the neighbours parent
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) //if neighbour is not walkable or on the closed set skip it
                        continue;

                    int movenCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour) + neighbour.movmentPenalty; //the cost for moving to the neighbour
                    if (movenCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) //if the new cost is lower than the old or neighbour has not yet been added to openset
                    {
                        neighbour.gCost = movenCostToNeighbour; //set g cost
                        neighbour.hCost = getDistance(neighbour, targetNode); //set h cost
                        neighbour.parent = currentNode; //set parent to backtrack to start from goal

                        if (!openSet.Contains(neighbour)) //if node was not in openset, add it
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour); //else, just update heap
                    }
                }
            }
        yield return null;
        if (pathSuccess)
        {
            UnityEngine.Debug.Log("this works");
            waypoints = RetracePath(startNode, targetNode); //if path was found, save it in the array
        }
        requsetManager.FinishedProcessingPath(waypoints, pathSuccess); //tell request manager that the path is found
    }
    Vector3[] RetracePath(Node firstNode, Node endNode) //gets the path by backtracking with the nodes parents 
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

    Vector3[] SimplyfyPath(List<Node> path) //only use nodes that change the direction in the path
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

    int getDistance(Node nodeA, Node nodeB) //gets the distance between two nodes by checking how many linear and diagonal steps are required
    {
        int x = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int y = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (x > y)
            return (x - y) * 10 + y * 14;
        return (y - x) * 10 + x * 14;
    }
}
