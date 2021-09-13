using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    [SerializeField]
    Transform arm;

    // Start is called before the first frame update
    void Start()
    {
        arm.RotateAround(transform.position, transform.root.right, -60);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
