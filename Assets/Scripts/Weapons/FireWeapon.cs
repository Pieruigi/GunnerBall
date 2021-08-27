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

        [SerializeField]
        GameObject bullet;

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
    }

}
