using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    
    public class Ball : MonoBehaviourPunCallbacks, IPunObservable, IHittable
    {

        public static Ball Instance { get; private set; }

        [Header("Materials")]
        [SerializeField]
        Material blueMaterial;

        [SerializeField]
        Material redMaterial;

        [SerializeField]
        int materialId;

        [Header("Fx")]
        [SerializeField]
        GameObject hitBlueParticlePrefab;

        [SerializeField]
        GameObject hitRedParticlePrefab;

        [SerializeField]
        ParticleSystem electricParticle;

        [SerializeField]
        ParticleSystem trailParticlePrefab;

        [SerializeField]
        GameObject explosionPrefab;

        [Header("Audio")]
        [SerializeField]
        AudioSource bounceAudioSource;

        [SerializeField]
        AudioSource explosionAudioSource;

        [SerializeField]
        AudioSource rollingAudioSource;


        Material defaultEmission;
        Renderer rend;
        

        Rigidbody rb;
        float drag = 0;

        Vector3 networkDisplacement = Vector3.zero;
        Vector3 networkPosition;
        Quaternion networkRotation;
        Vector3 networkVelocity;
        double networkTime;
        float lerpSpeed = 20;
        //Vector3 expectedPosition;
        bool grounded = false;
        DateTime groundedStartTime;
        float groundedTime = 0.5f;
        float rollingVolumeDefault;

        Collider coll;
        float radius; // Collider radius

        #region trail fields
        float trailMinSpeed = 10;
        bool trailForcedStop = false;
        ParticleSystem trailParticle;
        #endregion

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                rb = GetComponent<Rigidbody>();
                coll = GetComponent<Collider>();
                radius = ((SphereCollider)coll).radius;

                rend = GetComponentInChildren<Renderer>();
                defaultEmission = rend.materials[materialId];

                // Reset the trail particle parent
                if (trailParticlePrefab)
                {
                    trailParticle = GameObject.Instantiate(trailParticlePrefab);
                    trailParticle.transform.parent = null;
                }
                    

                if (rollingAudioSource)
                {
                    rollingVolumeDefault = rollingAudioSource.volume;
                    rollingAudioSource.volume = 0;
                    //rollingAudioSource.pitch = 0;
                    // Always playing, we just adjust the volume
                    rollingAudioSource.Play();
                }
                    
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
            
            if (rollingAudioSource)
            {
                //bool rolling = true;
                float rollingVolume = 0;
                float rollingMax = 36;
                // Apply rolling fx
                // Check if the ball collides with the floor
                if (grounded && (DateTime.UtcNow - groundedStartTime).TotalSeconds > groundedTime)
                {
                    // Check horizontal speed
                    Vector3 vel = rb.velocity;
                    vel.y = 0;
                   
                    
                    rollingVolume = Mathf.Lerp(0, rollingVolumeDefault, vel.sqrMagnitude / rollingMax);
                    
                }

                rollingAudioSource.volume = rollingVolume;
                //rollingAudioSource.pitch = rollingVolume;

            }
            

        }

        void LateUpdate()
        {
            // Trail
            if (trailParticle)
            {
                trailParticle.transform.position = transform.position;

                if (rb.velocity.magnitude < trailMinSpeed)
                {
                    if (trailParticle.isPlaying)
                        trailParticle.Stop();
                }
                else
                {
                    if (!trailParticle.isPlaying && !trailForcedStop)
                        trailParticle.Play();
                }
            }
            
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

                int mask = ~LayerMask.NameToLayer(Layer.Ball);
                if (Physics.CheckSphere(rb.position, radius, mask))
                {
                    SkipLastMasterClientSync();
                }
            
            }
            

        }

        /// <summary>
        /// Called on every client
        /// </summary>
        /// <param name="hitOwner"></param>
        /// <param name="hitPoint"></param>
        /// <param name="hitNormal"></param>
        /// <param name="hitDirection"></param>
        /// <param name="hitPower"></param>
        public void Hit(GameObject hitOwner, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection, float hitPower) 
        {
            Debug.LogFormat("Ball - hit power:" + hitPower);

            // Change the ball emission color depending on the team the player
            // who hit the ball belongs to.
            Team ownerTeam;
            if (PhotonNetwork.OfflineMode)
            {
                ownerTeam = Team.Blue;
            }
            else
            {
                ownerTeam = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(hitOwner.GetComponent<PhotonView>().Owner, PlayerCustomPropertyKey.TeamColor);
            }
      
            // Get the material by team
            Material tmp = null;
            
            
            switch (ownerTeam)
            {
                case Team.Red:
                        tmp = redMaterial;
                    break;
                case Team.Blue:
                        tmp = blueMaterial;
                    break;
            }
            // If the current material is not the right one then change it
            if (rend.materials[materialId] != tmp)
            {
                Material[] mats = rend.materials;
                mats[materialId] = tmp;
                rend.materials = mats;
            }

            // Set the hit particle
            GameObject hitPrefab = ownerTeam == Team.Blue ? hitBlueParticlePrefab : hitRedParticlePrefab;
            GameObject.Instantiate(hitPrefab, hitPoint, Quaternion.identity);

            // Play particle
            PlayElectricParticle();

            // Play audio
            //audioSource.Play();

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

            // Apply torque
            Vector3 cross = Vector3.Cross(hitNormal, hitDirection);
            rb.AddRelativeTorque(cross, ForceMode.Impulse);

            // Just skip the last sync from the master client
            SkipLastMasterClientSync();
        }

        

        public void ResetBall()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = LevelManager.Instance.BallSpawnPoint.position;
            rb.rotation = LevelManager.Instance.BallSpawnPoint.rotation;

            coll.enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;

            //trail.ForceStop(false);
            trailForcedStop = false;

            SkipLastMasterClientSync();
        }

        public void Explode()
        {
            coll.enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Apply fx
            GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Stop other fx
            electricParticle.Stop();
            if (trailParticle)
            {
                trailParticle.Stop();
                trailForcedStop = true;
            }
            

            // Play sount
            explosionAudioSource.Play();
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
                //if (Vector3.SqrMagnitude(position - rb.position) > 0.00000001f || Vector3.SqrMagnitude(velocity - rb.velocity) > 0.00000001f)
                //{
                    //Debug.LogFormat("Synch: vel:{0}, pos:{1}, rb.vel:{2}, rb.pos:{3}", velocity.y, position.y, rb.velocity.y, rb.position.y);
                    Synchronize(timestamp, position, rotation, velocity);
                    
                //}
                    
            }

        }



        #region private
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
            // Net code
            SkipLastMasterClientSync();

            // Play particle
            PlayElectricParticle();

            // Play audio
            bounceAudioSource.Play();

            
            // If ball collides with the floor then we set grounded on
            if (Layer.Ground.Equals(LayerMask.LayerToName(collision.gameObject.layer)))
            {
                grounded = true;
                groundedStartTime = DateTime.UtcNow;
            }
                
        }

        private void OnCollisionExit(Collision collision)
        {
            // If ball collides with the floor then we set grounded on
            if (Layer.Ground.Equals(LayerMask.LayerToName(collision.gameObject.layer)))
                grounded = false;
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

        void CheckPlayersCollision()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, (coll as SphereCollider).radius + 0.5f);
            if (colls != null)
            {
                foreach (Collider c in colls)
                {
                    if (Tag.Player.Equals(c.gameObject.tag))
                    {
                        //Debug.LogFormat("PlayerController - Collision with the ball.");
                        // Send hit for each player, but only the local player will take care
                        // of the health; anyway the others can apply some effect or stop
                        // moving in order to improve synchronization.
                        c.gameObject.GetComponent<PlayerController>().Hit(Ball.Instance.gameObject, Vector3.zero, Vector3.zero, Vector3.zero, 1000);
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
                
                //if(networkVelocity.y != 0)
                //{
                    // The ball is not grounded so we add gravity 
                    expectedVelocity = (expectedVelocity + Physics.gravity * Time.fixedDeltaTime) * (1 - rb.drag * Time.fixedDeltaTime);
                //}
                //else
                //{
                //    // The ball is grounded, we don't apply gravity
                //    expectedVelocity = expectedVelocity  * (1 - rb.drag * Time.fixedDeltaTime);

                //}
                
                expectedPosition += expectedVelocity * Time.fixedDeltaTime;
            }
            
            networkDisplacement = expectedPosition - rb.position;
            rb.velocity = expectedVelocity;
            
            //if (networkVelocity.sqrMagnitude > 0)
            //{
               
                // The ball is moving, we must check if is going to hit some collider; in that
                // case we skip the last sync and reset velocity
                //coll.enabled = false;
                Ray ray = new Ray(rb.position, networkDisplacement.normalized);
                int mask = ~LayerMask.NameToLayer(Layer.Ball);

                if (Physics.SphereCast(ray, (coll as SphereCollider).radius, networkDisplacement.magnitude, mask))
                {
                    SkipLastMasterClientSync();

                    rb.velocity = oldVelocity;
                    // If the ball was grounded we need to update the y velocity
                    if (networkVelocity.y == 0)
                    {

                        rb.position = new Vector3(rb.position.x, networkPosition.y, rb.position.z);
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    }

                }

                //coll.enabled = true;
            //}
            //else
            //{
               
            //    // The ball is not moving at all, so we just keep the position
            //    rb.position = networkPosition;
            //    networkDisplacement = Vector3.zero;
            //}
            
            
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

        void PlayElectricParticle()
        {
            if (electricParticle.isPlaying)
                return;

            electricParticle.Play();
        }

        #endregion

        #region rpc

        [PunRPC]
        public void RpcHitByPlayer(Vector3 newVelocity, double timestamp)
        {
            StartCoroutine(HitByPlayerDelayed(newVelocity, timestamp));
        }

        #endregion


    }

}
