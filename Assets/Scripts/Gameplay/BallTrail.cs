using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class BallTrail : MonoBehaviour
    {
        float minSpeed = 10f;

        Vector3 lastPosition;
        ParticleSystem ps;

        bool forcedStop = false;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            //ps.Stop();
        }

        // Start is called before the first frame update
        void Start()
        {
            transform.position = Ball.Instance.transform.position;
            lastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!Ball.Instance)
                return;

            transform.position = Ball.Instance.transform.position;
            
            // Check speed
            if(Vector3.Magnitude(transform.position - lastPosition)/Time.deltaTime < minSpeed)
            {
                if (ps.isPlaying)
                    ps.Stop();
            }
            else
            {
                if (!ps.isPlaying && !forcedStop)
                    ps.Play();
            }
            
            lastPosition = transform.position;
        }

        public void ForceStop(bool forced)
        {
            forcedStop = forced;
            if(forced)
                ps.Stop();
        }
    }

}
