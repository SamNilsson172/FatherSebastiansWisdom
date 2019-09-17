//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TargetReached : MonoBehaviour
//{
//    public Transform newTarget;

//    private void OnTriggerEnter(Collider collision)
//    {
//        if (collision.transform.tag == "Player")
//        {
//            Unit unit = collision.GetComponent<Unit>();
//            unit.targetIndex = 0;
//            unit.target = newTarget;
//            unit.FindTarget();
//        }
//    }
//}
