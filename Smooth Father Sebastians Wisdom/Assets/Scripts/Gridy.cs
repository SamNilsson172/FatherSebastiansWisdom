using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Gridy : MonoBehaviour
{
    public bool displayGridGizmos; //enable to see node gizmos
    public LayerMask unwalkableMask; //layer mask for obstacles
    public Vector2 gridWorldSize; //size of grid
    public float nodeRadius; //size of singel node
    public TerrainType[] walkableRegions; //array for all different terrains
    LayerMask walkableMask; //layer mask for all walkable surfaces
    Dictionary<int, int> walkableRegionDictionaty = new Dictionary<int, int>(); //dictionary for all terrain

    Node[,] grid; //2d array with all nodes ingrid

    float nodeDiameter; //less messy code
    int gridSizeX, gridSizeY; //amount of nodes in the x and y axis

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainType.value; //add all terrains layer masks to walkable mask
            walkableRegionDictionaty.Add((int) Mathf.Log(region.terrainType, 2), region.terrainPenalty); //add index of terrain and its penalty to dictonary
        }

        CreateGrid(); //fills the grid with nodes and add the correct values to them
    }

    public int MaxSize //how many nodes the grid contains
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; //fill grid with correct amount of empty nodes
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //define the bottom left of the grid

        for (int x = 0; x < gridSizeX; x++) //loop for all nodes in x axis
        {
            for (int y = 0; y < gridSizeY; y++) //loop for all nodes in y axis
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); //get position of each node
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); //check if node contains an obstacle

                int movmentPenalty = 0;
                if (walkable) //use a raycast to tell what terrain node is of
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionDictionaty.TryGetValue(hit.collider.gameObject.layer, out movmentPenalty); //get the movment penalty of nodes terrain
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movmentPenalty); //assign collected values to corresponding node
            }
        }
    }

    public List<Node> GetNeighbours(Node node) //get a list of nodes surrounding the parameter node
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++) //loop 9 times for all the nodes surrounding it and the node it self
            {
                if (x == 0 && y == 0) //skip node given in parameter
                    continue;

                int checkX = node.gridX + x; //get index of current node
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) //if node is not out of index add it to the neighbour list
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition) //find node to corresponding vector
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; //get position of corresponding node in precent
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; //add half the grid worldsize to adjust for the fact that the vector could be negative
        percentX = Mathf.Clamp01(percentX); //if vector is outside of grid, return node that is in a corner, instead of out of index 
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); //get index of node by multiplying the percent in the axis with the amount of nodes
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY); //round to int because index is an int and subtract 1 because index can be 0
        return grid[x, y];  
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType //class to terrains
    {
        public LayerMask terrainType; //the terrains layer mask
        public int terrainPenalty; //how taxing it is to walk through the terrain
    }
}

