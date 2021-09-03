#define PEER_SHOT
using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    /// <summary>
    /// Generic fire weapon class.
    /// Fire weapon is not networked, so RPC gets called on the player controller; the fire 
    /// weapon only need to set shooting parameters when ready to shot.
    /// </summary>
    public class FireWeapon : MonoBehaviour
    {
        [SerializeField]
        float power = 5f;

        [SerializeField]
        float speed = 60;

        [SerializeField]
        float damage = 10;

        [SerializeField]
        float fireRate = 0.5f;

        [SerializeField]
        float distance = 10;


        float shootDelay = 0.1f;

        float cooldown;
        float cooldownElapsed;

        PlayerController owner;
        Collider ownerCollider;
        


        private void Awake()
        {
            cooldown = 1f / fireRate;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Recall
            if(cooldownElapsed > 0)
            {
                cooldownElapsed -= Time.deltaTime;
            }


        }

#if !PEER_SHOT
        /// <summary>
        /// Returns true if it can shoot and some values are sent as param ( origin, speed etc );
        /// otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(out object[] parameters)
        {
            //Debug.Log("FireWeapon - TryShoot().");
            parameters = null;

            // Not ready yet
            if (cooldownElapsed > 0)
                return false;

            // Ready to shoot
            cooldownElapsed = cooldown;

            // Get origin, direction and speed
            Vector3 origin = owner.PlayerCamera.transform.position;
            Vector3 direction = owner.PlayerCamera.transform.forward;

            parameters = new object[3];
            parameters[0] = origin;
            parameters[1] = direction;
            parameters[2] = PhotonNetwork.Time;

            return true;
        }
#else
        /// <summary>
        /// Returns true if it can shoot and some values are sent as param ( origin, speed etc );
        /// otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(out object[] parameters)
        {
            //Debug.Log("FireWeapon - TryShoot().");
            parameters = null;
            
            // Not ready yet
            if (cooldownElapsed > 0)
                return false;

            // Ready to shoot
            cooldownElapsed = cooldown;

            // Get origin, direction and speed
            Vector3 origin = owner.PlayerCamera.transform.position;
            Vector3 direction = owner.PlayerCamera.transform.forward;

            // Check for collision
            Ray ray = new Ray(origin, direction);
            RaycastHit info;
            ownerCollider.enabled = false;
            bool hit = Physics.Raycast(ray, out info, distance);
            ownerCollider.enabled = true;
            if (hit)
            {
                //hitCollider = info.collider;

                if (Tag.Ball.Equals(info.collider.tag))
                {
                    parameters = new object[4];
                    parameters[0] = info.collider.GetComponent<Ball>().photonView.ViewID;
                    parameters[1] = info.point;
                    parameters[2] = info.normal;
                    parameters[3] = PhotonNetwork.Time;
                    
                }

                
            }
            
            return true;
        }
#endif




        //}

        public void SetOwner(PlayerController owner)
        {
            this.owner = owner;
            ownerCollider = owner.GetComponent<Collider>();
        }


        public void Shoot(object[] parameters)
        {
            StartCoroutine(ShootDelayed(parameters));
        }


#region private

#if !PEER_SHOT
        IEnumerator ShootDelayed(object[] parameters)
        {
            // Get params
            Vector3 origin = (Vector3)parameters[0];
            Vector3 direction = (Vector3)parameters[1];
            double timestamp = (double)parameters[2];

            // Check the time passed
            float lag = (float)(PhotonNetwork.Time - timestamp);
            if (shootDelay > lag)
                yield return new WaitForSeconds(shootDelay - lag);

            // Shoot


            if (PhotonNetwork.IsMasterClient)
            {

                // Cast a ray from the origin along the direction received
                Ray ray = new Ray(origin, direction);
                RaycastHit info;
                ownerCollider.enabled = false;
                bool hit = Physics.Raycast(ray, out info, distance);
                ownerCollider.enabled = true;
                if (hit)
                {
                   
                    if(Tag.Ball.Equals(info.collider.tag))
                    {
                        Debug.LogFormat("FireWeapon - Ball Hit.");
                        Vector3 position = info.transform.position;
                        Quaternion rotation = info.transform.rotation;
                        Vector3 velocity = -info.normal * power;
                        double ts = PhotonNetwork.Time;

                        // Change velocity
                        info.collider.GetComponent<Rigidbody>().velocity += velocity;

                        info.collider.GetComponent<Ball>().photonView.RPC("RpcHit", RpcTarget.Others, velocity, ts);

                   
                    }

                }

            }
        }
#else
        IEnumerator ShootDelayed(object[] parameters)
        {
            // We can add some fx here

            if (parameters == null)
            {
                yield break;
            }


            // Get params
            PhotonView photonView = PhotonNetwork.GetPhotonView((int)parameters[0]);
            Vector3 hitPoint = (Vector3)parameters[1];
            Vector3 hitNormal = (Vector3)parameters[2];
            double timestamp = (double)parameters[3];

            // Check the time passed
            float lag = (float)(PhotonNetwork.Time - timestamp);
            if (shootDelay >= lag)
                yield return new WaitForSeconds(shootDelay - lag);
            //else
            //    yield break; // It took to much time

            // Shoot
            if(photonView != null)
            {
                IHittable hittable = photonView.gameObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    hittable.Hit(owner.gameObject, hitPoint, hitNormal, power);
                }
            }


            //if (Tag.Ball.Equals(photonView.tag))
            //{
                

            //    //// We want the ball to get moved on all the clients in order
            //    //// to have a very smooth movement
            //    //Vector3 velocity = -hitNormal * power;
            //    //Ball.Instance.GetComponent<Rigidbody>().velocity += velocity;

            //    //// Just skip the last sync from the master client
            //    //Ball.Instance.SkipLastMasterClientSync();

            //    Ball.Instance.Hit(owner.gameObject, hitPoint, hitNormal, power);
            //}
            
            //if(PhotonNetwork.IsMasterClient)
            //    Ball.Instance.photonView.RPC("RpcHit", RpcTarget.Others, velocity, ts);

    
               

        }
#endif

#endregion
    }

}
