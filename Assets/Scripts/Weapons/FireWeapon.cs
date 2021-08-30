//#define SHOOT_AS_PEER
using Photon.Pun;
using System.Collections;
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

        /// <summary>
        /// Returns true if it can shoot and some values are sent as param ( origin, speed etc );
        /// otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(out object[] parameters)
        {
            Debug.Log("FireWeapon - TryShoot().");
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
            ownerCollider = owner.GetComponent<Collider>();
        }


        public void Shoot(object[] parameters)
        {
            StartCoroutine(ShootDelayed(parameters));
        }


        #region private

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
                        Vector3 velocity = info.normal * -3 * power;
                        double ts = PhotonNetwork.Time;

                        // Change velocity
                        info.collider.GetComponent<Rigidbody>().velocity += velocity;

                        info.collider.GetComponent<Ball>().photonView.RPC("RpcHit", RpcTarget.Others, velocity, ts);

                   
                }

            }

        }
     }


#endregion
    }

}
