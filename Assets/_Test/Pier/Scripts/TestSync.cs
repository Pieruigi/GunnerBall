using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSync : MonoBehaviour
{
    TestBall ball;

    GameObject cloneBall;

    // Start is called before the first frame update
    void Start()
    {
        int ticks = (int)(1.12f / 0.02f);
        Debug.Log("Ticks:" + ticks);


        ball = GameObject.FindObjectOfType<TestBall>();
        cloneBall = GameObject.Instantiate(ball.gameObject);
        cloneBall.GetComponent<Rigidbody>().position = ball.GetComponent<Rigidbody>().position + Vector3.forward * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SyncBall(Vector3 position, Vector3 velocity, float timestamp)
    {
        Debug.Log("Ball synchronization");
        StartCoroutine(DoSyncBall(position, velocity, timestamp));
    }

    IEnumerator DoSyncBall(Vector3 position, Vector3 velocity, float timestamp)
    {
        yield return new WaitForSeconds(.3f);

        cloneBall.GetComponent<TestBall>().ReceiveSync(position, velocity, timestamp);

        Rigidbody orb = ball.GetComponent<Rigidbody>();
        Rigidbody rb = cloneBall.GetComponent<Rigidbody>();

        //// Total time passed since the rpc call
        //float timeLag = Time.time - timestamp;
        //Debug.Log("Time lag: " + timeLag);
        //// How many ticks in the time lag ?
        //int numOfTicks = (int)(timeLag / Time.fixedDeltaTime);
        
        //// Compute velocity
        //float drag = Mathf.Pow(1f - Time.fixedDeltaTime * TestBall.Drag, numOfTicks);
        //Vector3 newVel = velocity * drag;
        Debug.LogFormat("Original ball velocity: {0},{1},{2}", orb.velocity.x, orb.velocity.y, orb.velocity.z);
        Debug.LogFormat("Cloned ball velocity: {0},{1},{2}", rb.velocity.x, rb.velocity.y, rb.velocity.z);

        // Get new position
        // Each tick has a constant speed, so we can simply calculate the ball displacement
        // for each tick and then sum all the values togheter
        //Vector3 disp = Vector3.zero;
        //for (int i=0; i< numOfTicks; i++)
        //{
        //    Vector3 vel = velocity * Mathf.Pow(1f - Time.fixedDeltaTime * TestBall.Drag, i+1);
        //    disp += vel * Time.fixedDeltaTime;
        //    // Drag velocity for the next tick
            
        //}
        //Vector3 newPos = position + disp;
     
        Debug.LogFormat("Original ball pos: {0},{1},{2}", orb.position.x, orb.position.y, orb.position.z);
        Debug.LogFormat("Cloned ball pos: {0},{1},{2}", rb.position.x, rb.position.y, rb.position.z);

        //rb.position = rb.position + Vector3.forward * 2f;
        //rb.velocity = newVel;

    }
}
