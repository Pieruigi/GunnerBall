using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGravity : MonoBehaviour
{
    [SerializeField]
    bool custom;

    

    Rigidbody rb;
    float startY;
    float drag = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startY = rb.position.y;

        if (custom)
        {
            rb.useGravity = false;
            drag = rb.drag;
            rb.drag = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.LogFormat("Time: {0}, Velocity.Y: {1}, Y:{2}", Time.time, rb.velocity.y, startY - rb.position.y);
        if(custom)
        {
            rb.velocity = (rb.velocity + Physics.gravity * Time.fixedDeltaTime) * (1 - drag * Time.fixedDeltaTime);

        }
        


    }
}
