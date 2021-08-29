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
        float lerpSpeed = 20;


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
          
        }

        private void FixedUpdate()
        {
            //Debug.LogFormat("PhotonNetwork.Ping():" + PhotonNetwork.GetPing());

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
                //rb.MovePosition(rb.position + deltaDisp);
                

            }
         

        }

       

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //return;
            

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

                Debug.LogFormat("Ball - Sync() ........................");
                Debug.LogFormat("Ball - Sync() Timestamp: {0}", timestamp);
                Debug.LogFormat("Ball - Sync() Position: {0}", position);
                Debug.LogFormat("Ball - Sync() Velocity: {0}", velocity);
                Debug.LogFormat("Ball - Sync() completed ........................");
                Synchronize(timestamp, position, rotation, velocity);
            }
        }

        [PunRPC]
        void RpcHit(Vector3 velocity, double timestamp)
        {
            Debug.LogFormat("Ball - RpcHit() ....................");
            Debug.LogFormat("Ball - RpcHit() Timestamp: {0}", timestamp);
            Debug.LogFormat("Ball - RpcHit() Velocity: {0}", velocity);
            Debug.LogFormat("Ball - RpcHit() completed ....................");

           

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - timestamp));

            rb.velocity = velocity + Physics.gravity * lag;
            
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
            

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - networkTime));

            networkDisplacement = (Physics.gravity / 2.0f) * Mathf.Pow(lag,2) + networkPosition + networkVelocity * lag - rb.position;
            //networkDisplacement += (Physics.gravity / 2.0f) * lag * lag;
            rb.velocity = networkVelocity + Physics.gravity * lag;

            lerpSpeed = networkDisplacement.magnitude / 0.08f;

            //rb.position += networkDisplacement;
            //rb.MovePosition(rb.position + networkDisplacement);
            //networkDisplacement = Vector3.zero;

        }

    }

}
