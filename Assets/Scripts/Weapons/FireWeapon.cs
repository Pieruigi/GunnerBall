using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float mass = 5f;

        [SerializeField]
        float speed = 60;

        [SerializeField]
        float damage = 10;

        [SerializeField]
        float fireRate = 0.5f;

        [SerializeField]
        float distance = 10;

#if SHOOT_BULLET
        [SerializeField]
        GameObject bullet;
#else
        float shootDelay = 0.2f;
#endif
        float cooldown;
        float cooldownElapsed;

        PlayerController owner;

        


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

        /// <summary>
        /// Returns true if it can shoot and some values are sent as param ( origin, speed etc );
        /// otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(out object[] parameters)
        {
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

        
            


        //}

        public void SetOwner(PlayerController owner)
        {
            this.owner = owner;
        }

#if SHOOT_BULLET
        public virtual void Shoot(object[] parameters)
        {
            // Get params
            Vector3 origin = (Vector3)parameters[0];
            Vector3 direction = (Vector3)parameters[1];
            double timestamp = (double)parameters[2];

            // Create the bullet
            GameObject obj = Instantiate(bullet, Vector3.zero, Quaternion.identity);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            // Lag compensation
            rb.position = origin + speed * (float)(PhotonNetwork.Time - timestamp) * direction;
            // Set velocity
            rb.velocity = direction * speed;

            // Remove collision between bullet and its owner
            Physics.IgnoreCollision(owner.GetComponent<CharacterController>(), obj.GetComponent<Collider>(), true);
        }
#else
        public void Shoot(object[] parameters)
        {
            StartCoroutine(ShootDelayed(parameters));
        }
#endif

        #region private
#if !SHOOT_BULLET
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
                if(Physics.Raycast(ray, out info, distance))
                {
                   
                    if(Tag.Ball.Equals(info.collider.tag))
                    {
                        Vector3 position = info.transform.position;
                        Quaternion rotation = info.transform.rotation;
                        Vector3 velocity = info.normal * -3;
                        double ts = PhotonNetwork.Time;

                        // Change velocity
                        info.collider.GetComponent<Ball>().photonView.RPC("RpcHit", RpcTarget.All, position, rotation, velocity, ts);
                        // I'm updating other clients via rpc, so no other syncs are needed
                        // for 0.1 sec
                        info.collider.GetComponent<Ball>().DelaySynchronization();
                    }
                    
                }
            }
        }

#endif
#endregion
    }

}
