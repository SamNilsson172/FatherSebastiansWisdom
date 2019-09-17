using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    float speed = 20;
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound); //makes request for a path with all info needed for it
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) //magic
    {
        if (pathSuccessful)
            path = newPath;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath() //basiclly same as Update but can be started and stoped. for moving to target along path
    {
        Vector3 currentWaypoint = path[0]; //current waypoint is what seeker is currently moving towards, set it to starting point in begining

        while (true)
        {
            if (transform.position == currentWaypoint) //if seeker has reached what it's moving towards
            {
                targetIndex++; //increase to move to new position
                if (targetIndex >= path.Length) //if you went through all positions in path,target reached
                    yield break;
                currentWaypoint = path[targetIndex]; //set current goal to new position
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime); //move
            yield return null; //wait one frame
        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);
                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
