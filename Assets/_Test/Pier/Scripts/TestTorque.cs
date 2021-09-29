using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTorque : MonoBehaviour
{
    [SerializeField]
    Transform direction;

    [SerializeField]
    Transform normal;

    Rigidbody rb;



    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyTorque()
    {

        Vector3 cross = Vector3.Cross(normal.forward, direction.forward);
        Debug.Log("Cross:" + cross);
        rb.AddRelativeTorque(cross, ForceMode.VelocityChange);
    }
}
