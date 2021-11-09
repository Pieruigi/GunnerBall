using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour
{
    [SerializeField]
    Transform origin;

    [SerializeField]
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetAngle();
    }

    float GetAngle()
    {
        float angle = 0;

        Vector3 originToTarget = target.position - origin.position;
        Vector3 planeNormal = origin.right;
        
        Debug.DrawRay(origin.position, planeNormal.normalized*5, Color.red);

        Vector3 originToTargetProj = Vector3.ProjectOnPlane(originToTarget, planeNormal);
        Debug.DrawRay(originToTargetProj, originToTargetProj.normalized * 5, Color.green);
        //Vector3 fwdProj = Vector3.ProjectOnPlane(origin.forward, planeNormal);
        
        angle = Vector3.SignedAngle(origin.forward, originToTargetProj, planeNormal);
        Debug.Log("Angle:" + angle);
        return angle;
    }
}
