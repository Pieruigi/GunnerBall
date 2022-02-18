using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class Magnet : MonoBehaviour
    {
        [SerializeField]
        float forcePower;

        [SerializeField]
        float lifeTime;

        Rigidbody ballRB;


        // Start is called before the first frame update
        void Start()
        {
            ballRB = GameObject.FindGameObjectWithTag(Tag.Ball).GetComponent<Rigidbody>();

            Destroy(gameObject, lifeTime);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            // Compute direction
            Vector3 dir = transform.position - ballRB.position;


            ballRB.AddForce(dir.normalized * forcePower, ForceMode.Force);
        }


    }

}
