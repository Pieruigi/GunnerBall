using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    
    public class Ball : MonoBehaviourPunCallbacks, IPunObservable
    {

        Rigidbody rb;
        float drag = 0;

        Vector3 networkDisplacement = Vector3.zero;
        Vector3 networkPosition;
        Quaternion networkRotation;
        Vector3 networkVelocity;
        double networkTime;
        float lerpSpeed = 10;

        // When you hit the ball the rpc is called via server; in the meantime the master client
        // might send a sync for the ball with an updated timestamp; so we must stop for 0.1 sec
        //bool delaySync = false;
        float delaySyncTime = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (delaySyncTime > 0)
                delaySyncTime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            

            //return;
            if (photonView.IsMine)
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
                

                //Vector3.MoveTowards(rb.position, rb.position + lerpDisp, Time.fixedDeltaTime*lerpSpeed);
                rb.position += deltaDisp;
                

            }
         

        }

        public void DelaySynchronization()
        {
            //delaySync = true;
            delaySyncTime = 0.1f;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //return;
            

            if (stream.IsWriting) // Local ( should be the MasterClient )
            {
                double time = delaySyncTime > 0 ? 0 : PhotonNetwork.Time;
               
                stream.SendNext(time);
                if (time == 0)
                    return;
                stream.SendNext(rb.position);
                stream.SendNext(rb.rotation);
                stream.SendNext(rb.velocity);
               
            }
            else
            {
                
               
                double timestamp = (double)stream.ReceiveNext();
                if (timestamp == 0)
                    return;
               
                Vector3 position = (Vector3)stream.ReceiveNext();
                Quaternion rotation = (Quaternion)stream.ReceiveNext();
                Vector3 velocity = (Vector3)stream.ReceiveNext();


                Synchronize(timestamp, position, rotation, velocity);
            }
        }

        [PunRPC]
        void RpcHit(Vector3 position, Quaternion rotation, Vector3 velocity, double timestamp)
        {
            Debug.LogFormat("Ball - Receiving RpcHit() ...");
            // Delete previuos sync
            networkTime = 0;
            networkDisplacement = Vector3.zero;
            Synchronize(timestamp, position, rotation, velocity);

        }

        /// <summary>
        /// In case of collision the old network sync gets skipped.
        /// Immagine the master client sends a sync just before a collision happen; meanwhile the
        /// collision happens on the client and the ball moves accordingly; but the client 
        /// receives the sync ( or is already managing it lerping position ) and moves the ball 
        /// back again, because that sync has been computed before any collision.
        /// The next sync will take into account collision, moving the ball forward again ( that
        /// boring elastic movement you see without this code ).
        /// For the same reason we also reset the displacement.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!photonView.IsMine)
            {
                // Update network time to skip any sync that is just arrived
                networkTime = PhotonNetwork.Time;
                networkDisplacement = Vector3.zero;
            }
                
        }

        void Synchronize(double timestamp, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            double oldNetworkTime = networkTime;
            networkTime = timestamp;
            if (oldNetworkTime > networkTime)
            {
                networkTime = oldNetworkTime;
                return;
            }
            networkPosition = position;
            networkRotation = rotation;
            networkVelocity = velocity;
            Debug.LogFormat("Ball - Receiving sync [Time:{0}].", networkTime);

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - networkTime));

            networkDisplacement = networkPosition + networkVelocity * lag - rb.position;
            rb.velocity = networkVelocity;
        }

    }

}
