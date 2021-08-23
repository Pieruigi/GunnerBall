using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    //GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        //ball = GameObject.FindObjectOfType<TestBall>().gameObject;    
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = -3 * Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogFormat("Bullet has collided with " + collision.gameObject.name);
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);

    }
}
