using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    
    public class Ball : MonoBehaviourPunCallbacks, IPunObservable, IHittable
    {

        public static Ball Instance { get; private set; }

        Rigidbody rb;
        float drag = 0;

        Vector3 networkDisplacement = Vector3.zero;
        Vector3 networkPosition;
        Quaternion networkRotation;
        Vector3 networkVelocity;
        double networkTime;
        float lerpSpeed = 20;
        //Vector3 expectedPosition;

        Collider coll;
        float radius; // Collider radius

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                rb = GetComponent<Rigidbody>();
                coll = GetComponent<Collider>();
                radius = ((SphereCollider)coll).radius;

                
            }
            else
            {
                Destroy(gameObject);
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
            // If the player move towards the ball on collision might not be triggered, 
            // I guess because we are using character controller; so we check for 
            // collision with all the players here.
            CheckPlayersCollision();

            if (PhotonNetwork.IsMasterClient)
                return;

            if (networkDisplacement != Vector3.zero)
            {
                //float lerpSpeed = 10;
                float netDispMag = networkDisplacement.magnitude;
                float deltaDispMag = Time.fixedDeltaTime * lerpSpeed;
                Vector3 deltaDisp;
                if (netDispMag > deltaDispMag)
                {
                    deltaDisp = networkDisplacement.normalized * deltaDispMag;
                    networkDisplacement = networkDisplacement.normalized * (netDispMag - deltaDispMag);
                }
                else
                {
                    deltaDisp = networkDisplacement.normalized * netDispMag;
                    networkDisplacement = Vector3.zero;
                }
                
                rb.MovePosition(rb.position + deltaDisp);

                // Check for collisions
                coll.enabled = false;
                if (Physics.CheckSphere(rb.position, radius))
                {
                    //networkTime = PhotonNetwork.Time;
                    //networkDisplacement = Vector3.zero;
                    SkipLastMasterClientSync();
                }
                coll.enabled = true;
            }
         

        }

        public void Hit(GameObject hitOwner, Vector3 hitPoint, Vector3 hitNormal, float hitPower) 
        {
            Debug.LogFormat("Ball - hit");

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
            if(vComp > 0)
            {
                //velocity += -0.75f * vComp * hitNormal;
                velocity += -0.80f * vComp * hitNormal;
            }

            rb.velocity += velocity;

            // Just skip the last sync from the master client
            SkipLastMasterClientSync();
        }

        void SkipLastMasterClientSync()
        {
            if (!photonView.IsMine)
            {
                // Update network time to skip any sync that is just arrived
                networkTime = PhotonNetwork.Time;
                networkDisplacement = Vector3.zero;
            }

        }

        public void ResetBall()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = LevelManager.Instance.BallSpawnPoint.position;
            rb.rotation = LevelManager.Instance.BallSpawnPoint.rotation;

            coll.enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;

            SkipLastMasterClientSync();
        }

        public void Explode()
        {
            coll.enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {


            if (stream.IsWriting) // Local ( should be the MasterClient )
            {
              
               
                stream.SendNext(PhotonNetwork.Time);
                
                stream.SendNext(rb.position);
                stream.SendNext(rb.rotation);
                stream.SendNext(rb.velocity);
               
            }
            else
            {
                
               
                double timestamp = (double)stream.ReceiveNext();
               
               
                Vector3 position = (Vector3)stream.ReceiveNext();
                Quaternion rotation = (Quaternion)stream.ReceiveNext();
                Vector3 velocity = (Vector3)stream.ReceiveNext();

                //Debug.LogFormat("Ball - Sync() ........................");
                //Debug.LogFormat("Ball - Sync() Timestamp: {0}", timestamp);
                //Debug.LogFormat("Ball - Sync() Position: {0}", position);
                //Debug.LogFormat("Ball - Sync() Velocity: {0}", velocity);
                //Debug.LogFormat("Ball - Sync() completed ........................");
                Synchronize(timestamp, position, rotation, velocity);
            }

        }


       

        /// <summary>
        /// In case of collision the old network sync gets skipped.
        /// Immagine the master client sends a sync just before a collision happens; 
        /// meanwhile the collision also happens on the client and the ball gets moved
        /// accordingly; but the client receives the sync ( or is already managing it 
        /// lerping old position ) and moves the ball back again, because that sync 
        /// has been computed before any collision. 
        /// The next sync will take finally into account the collision, moving the ball 
        /// forward again giving it a boring elastic movement.
        /// For the same reason we also reset the displacement.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            //Debug.LogFormat("Ball - Collision detected: {0}", collision.gameObject);
            SkipLastMasterClientSync();
        }


        void CheckPlayersCollision()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, (coll as SphereCollider).radius + 0.5f);
            if (colls != null)
            {
                foreach (Collider c in colls)
                {
                    if (Tag.Player.Equals(c.gameObject.tag))
                    {
                        Debug.LogFormat("PlayerController - Collision with the ball.");
                        // Send hit for each player, but only the local player will take care
                        // of the health; anyway the others can apply some effect or stop
                        // moving in order to improve synchronization.
                        c.gameObject.GetComponent<PlayerController>().Hit(Ball.Instance.gameObject, Vector3.zero, Vector3.zero, 1000);
                    }
                }
            }
        }

        void Synchronize(double timestamp, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            // Skip old synchs
            double oldNetworkTime = networkTime;
            Vector3 oldVelocity = rb.velocity;
            networkTime = timestamp;
            if (oldNetworkTime > networkTime)
            {
                networkTime = oldNetworkTime; 
                return;
            }

            //            // Is it too old? Why I should skip it?
            //            if(PhotonNetwork.Time > networkTime + 0.165f)
            //            {
            //                // Too old, skip
            //                networkTime = oldNetworkTime;
            //                return;
            //            }

            // This should avoid strange bouncing when you shoot the ball
            // near walls.
            //if (PhotonNetwork.Time - networkTime < 0.04f)
            //{
            //    networkTime = oldNetworkTime;
            //    return;
            //}

            networkPosition = position;
            networkRotation = rotation;
            networkVelocity = velocity;
            

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - networkTime));
            int numOfTicks = (int)(lag / Time.fixedDeltaTime); // For physics computations

            Vector3 expectedPosition = networkPosition;
            Vector3 expectedVelocity = networkVelocity;

            // Adding gravity and drag
            // Velocity changes in a discrete mode, so we just need to calculate the displacement
            // for each physics tick
            for (int i=0; i<numOfTicks; i++)
            {
                expectedVelocity = (expectedVelocity + Physics.gravity * Time.fixedDeltaTime) * (1 - rb.drag * Time.fixedDeltaTime); 
                expectedPosition += expectedVelocity * Time.fixedDeltaTime;
            }

            networkDisplacement = expectedPosition - rb.position;
            rb.velocity = expectedVelocity;

            coll.enabled = false;
            Ray ray = new Ray(rb.position, networkDisplacement.normalized);
            if(Physics.SphereCast(ray, (coll as SphereCollider).radius, networkDisplacement.magnitude))
            {
           
                SkipLastMasterClientSync();
                rb.velocity = oldVelocity;
            }
            
            coll.enabled = true;

            //lerpSpeed = networkDisplacement.magnitude / 0.075f;
            lerpSpeed = 10f;

        }

        IEnumerator HitByPlayerDelayed(Vector3 newVelocity, double timestamp)
        {
            // Try to apply the new velocity at the same time using constant delay
            float delay = 0.1f;
            float lag = (float)(PhotonNetwork.Time - timestamp);
            if (delay >= lag)
                yield return new WaitForSeconds(delay - lag);

            // Apply new velocity
            rb.velocity = newVelocity;

            // Skip the current sync or anyother one coming 
            SkipLastMasterClientSync();
        }

        #region rpc

        [PunRPC]
        public void RpcHitByPlayer(Vector3 newVelocity, double timestamp)
        {
            StartCoroutine(HitByPlayerDelayed(newVelocity, timestamp));
        }

        #endregion


    }

}
