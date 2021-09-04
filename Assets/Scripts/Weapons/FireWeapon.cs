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

        [SerializeField]
        int coolerCount = 3;

        [SerializeField]
        int coolerCooldown = 4;

        float shootDelay = 0.1f;

        float cooldown;
        float cooldownElapsed;

        PlayerController owner;
        Collider ownerCollider;
        int activeCoolerCount;
        float coolerCooldownElapsed;

        private void Awake()
        {
            cooldown = 1f / fireRate;
            activeCoolerCount = coolerCount;
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

            // Reload coolers
            if(activeCoolerCount < coolerCount)
            {
                coolerCooldownElapsed -= Time.deltaTime;
                if(coolerCooldownElapsed < 0)
                {
                    activeCoolerCount++;
                    coolerCooldownElapsed = coolerCooldown;
                }
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
            //Debug.Log("FireWeapon - TryShoot().");
            parameters = null;
            
            // Not ready yet
            if (cooldownElapsed > 0)
                return false;

            // Check coolers
            if (activeCoolerCount == 0)
                return false;

            activeCoolerCount--;

            // Ready to shoot
            cooldownElapsed = cooldown;
            coolerCooldownElapsed = coolerCooldown;

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
   

        }


#endregion
    }

}
