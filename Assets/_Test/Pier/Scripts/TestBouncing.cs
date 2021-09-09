using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBouncing : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = Vector3.forward * 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.LogFormat("FixedUpdate - Velocity: {0}, {1}, {2}", rb.velocity.x, rb.velocity.y, rb.velocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogFormat("OnCollisionEnter - Velocity: {0}, {1}, {2}", rb.velocity.x, rb.velocity.y, rb.velocity.z);
    }
}
