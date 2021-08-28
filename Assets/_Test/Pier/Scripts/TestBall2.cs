using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall2 : MonoBehaviour
{
    [SerializeField]
    Vector3 velocity;

    [SerializeField]
    Vector3 displacement;

    [SerializeField]
    float lerpSpeed;

    Rigidbody rb;

    DateTime start;
    bool started;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = velocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
        if(displacement != Vector3.zero)
        {
            if (!started)
            {
                started = true;
                start = DateTime.UtcNow;
            }

            float deltaMag = Time.deltaTime * lerpSpeed;
            float dispMag = displacement.magnitude;
            Vector3 delta;
            if (deltaMag < dispMag)
            {
                delta = displacement.normalized * deltaMag;
                displacement = displacement.normalized * (dispMag - deltaMag);
            }
            else
            {
                delta = displacement;
                displacement = Vector3.zero;
            }

            rb.position += delta;
        }
        else
        {
            if (started)
            {
                started = false;
                Debug.LogFormat("Time elapsed: {0}", (DateTime.UtcNow-start).TotalMilliseconds);
            }
        }
    }
}
