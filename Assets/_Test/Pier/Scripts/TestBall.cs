using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour
{

    public static readonly float Drag = 1;

    [SerializeField]
    bool testDrag = false;

    Rigidbody rb;

    Vector3 lerpDisp = Vector3.zero;
    
    TestSync sync;
    bool setSafePosition = false;
    Vector3 safePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sync = GameObject.FindObjectOfType<TestSync>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (setSafePosition)
        {
           // setSafePosition = false;
            rb.position = safePosition;
        }

        if (lerpDisp != Vector3.zero)
        {
            float lerpSpeed = 3;
            float lerpDispMag = lerpDisp.magnitude;
            float disp = Time.fixedDeltaTime * lerpSpeed;
            if (lerpDispMag > disp)
            {
                lerpDispMag -= disp;
            }
            else
            {
                disp = lerpDispMag;
                lerpDispMag = 0;
            }


            //Vector3.MoveTowards(rb.position, rb.position + lerpDisp, Time.fixedDeltaTime*lerpSpeed);
            rb.position = rb.position + lerpDisp.normalized * disp;

            lerpDisp = lerpDisp.normalized * lerpDispMag;
        }

        CheckDrag();
    }

    /// <summary>
    /// Simulate the receiving RPC for update ball rigidbody
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="timestamp"></param>
    public void ReceiveSync(Vector3 position, Vector3 velocity, float timestamp)
    {
        // Total time passed since the rpc call
        float timeLag = Time.time - timestamp;
        Debug.Log("Time lag: " + timeLag);
        // How many ticks in the time lag ?
        int numOfTicks = (int)(timeLag / Time.fixedDeltaTime);

        // Compute velocity
        float drag = Mathf.Pow(1f - Time.fixedDeltaTime * TestBall.Drag, numOfTicks);
        Vector3 newVel = velocity * drag;
    
        // Get new position
        // Each tick has a constant speed, so we can simply calculate the ball displacement
        // for each tick and then sum all the values togheter
        Vector3 disp = Vector3.zero;
        for (int i = 0; i < numOfTicks; i++)
        {
            // Drag velocity for the current tick
            Vector3 vel = velocity * Mathf.Pow(1f - Time.fixedDeltaTime * TestBall.Drag, i + 1);
            // Compute the current ball displacement from the current tick velocity
            disp += vel * Time.fixedDeltaTime;
        }
        Vector3 newPos = position + disp;

        // We don't want to teleport the ball, so we use interpolation
        //rb.position = newPos + Vector3.forward * 2f;
        lerpDisp = newPos + Vector3.forward * 2f - rb.position;

        // Set new velocity
        rb.velocity = newVel;

        
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Ball has collided with " + collision.gameObject.name);
        Debug.LogFormat("Ball starting position after collision: {0},{1},{2}", rb.position.x, rb.position.y, rb.position.z);
        Debug.Log("Ball new velocity after collision: " + rb.velocity);
        sync.SyncBall(rb.position, rb.velocity, Time.time);
        rb.velocity = Vector3.zero;
        safePosition = rb.position;
        setSafePosition = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Ball trigger enter:" + other.name);
        //other.
    }

    void CheckDrag()
    {
        if (testDrag)
        {
            //Debug.Log("FixedDelta:" + Time.fixedDeltaTime);
            rb.velocity *= (1 - Time.fixedDeltaTime * Drag);
            
            //// Drag calculation ( we need to calculate the drag factor for synch purpose )
            //float v = rb.velocity.magnitude;
            //if (v > 0)
            //{
            //    v = v * (1 - Time.fixedDeltaTime * drag);
            //    rb.velocity = rb.velocity.normalized * v;
            //}
        }
    }
}
