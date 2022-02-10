using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CastSphere();
        }
    }

    void CastSphere()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        float radius = 1;
        float distance = 8 - radius;
        RaycastHit info;
        Ray ray = new Ray(origin, direction);
        if(Physics.SphereCast(ray, radius, out info, distance))
        {
            Debug.LogFormat("Object hit: {0}", info.transform.gameObject);
            Debug.LogFormat("HitInfo - Point:{0}", info.point);
        }
        else
        {
            Debug.Log("No object hit");
        }
    }
}
