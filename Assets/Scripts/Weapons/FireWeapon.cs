using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class FireWeapon : MonoBehaviour
    {
        [SerializeField]
        float mass = 5f;

        [SerializeField]
        float speed = 20;

        [SerializeField]
        float damage = 10;

        [SerializeField]
        float fireRate = 0.5f;

        [SerializeField]
        float distance = 10;

        float cooldown;
        float cooldownElapsed;

        PlayerController owner;

        private void Awake()
        {
            cooldown = 1f / 0.5f;
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

        public bool TryShoot(out object[] parameters)
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

        
        private void Shoot()
        {

        }
    }

}
