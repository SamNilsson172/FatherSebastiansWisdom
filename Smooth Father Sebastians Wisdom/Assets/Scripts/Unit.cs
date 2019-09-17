using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform[] targets;
    Transform target;
    float speed = 200;
    Vector3[] path;
    public int targetIndex;
    int newTargetIndex = 0;

    private void Start()
    {
        target = targets[newTargetIndex];
        FindTarget();
    }

    public void FindTarget()
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
            if (Vector3.Distance(transform.position, currentWaypoint) <= 0.71f) //if seeker has reached what it's moving towards, or is half a node along the hypothanues away from it
            {
                targetIndex++; //increase to move to new position
                if (targetIndex >= path.Length) //if you went through all positions in path,target reached
                {
                    FindNextTarget();
                    yield break;
                }
                currentWaypoint = path[targetIndex]; //set current goal to new position
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime); //move
            yield return null; //wait one frame
        }
    }

    void FindNextTarget()
    {
        if (newTargetIndex++ >= targets.Length - 1)
            newTargetIndex = 0;
        target = targets[newTargetIndex];
        targetIndex = 0;
        FindTarget();
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
