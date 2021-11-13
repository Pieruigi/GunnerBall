using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBall : MonoBehaviour
{
    public static FakeBall Instance { get; private set; }

    Rigidbody rb;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = Vector3.right * 33;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 ComputeNewVelocity(float hitPower, Vector3 hitPoint, Vector3 hitNormal)
    {
        // We want the ball to move on all the clients in order
        // to have a very smooth movement
        // Momentum:
        // V1i: bullet initial velocity
        // V2i: ball initial velocity
        // V2f: ball final velocity
        // M1: bullet mass
        // M2: ball mass
        // Lets assume that M1 >> M2 ( M1 a lot bigger than M2 ):
        // then V2f = 2*V1i ( we assume the hit power is the buller speed )
        // and if the ball is moving towards the bullet then V2f = V2i, so we don't 
        // need to consider the bullet mass and we can assume to preserve lets 
        // say the 80% of component of the ball velocity along the hit normal
        Vector3 velocity = -hitNormal * hitPower;

        // We want to keep some energy if ball is coming towards us
        float vComp = Vector3.Dot(rb.velocity, hitNormal);
        //vComp = 0; ///////////// Test
        if (vComp > 0)
        {
            //velocity += -0.75f * vComp * hitNormal;
            velocity += -0.80f * vComp * hitNormal;
        }

        // Add the rigidbody velocity
        velocity += rb.velocity;
        return velocity;
    }
}
