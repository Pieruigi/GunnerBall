//#define TEST_DISTANCE
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
        [Header("Stats")]
        [SerializeField]
        float power = 5f;

        //[SerializeField]
        //float speed = 60;

        [SerializeField]
        float damage = 10;

        [SerializeField]
        float fireRate = 0.5f;
        public float FireRate
        {
            get { return fireRate; }
        }

        [SerializeField]
        float fireRange = 8.6f;
        public float FireRange
        {
            get { return fireRange; }
        }

        [Header("Fx")]
        [SerializeField]
        ParticleSystem shootParticle;

        [SerializeField]
        AudioSource audioSource;

        //[SerializeField]
        //int coolerCount = 3;

        //[SerializeField]
        //int coolerCooldown = 4;

        float shootDelay = 0.1f;

        float cooldown;
        public float Cooldown
        {
            get { return cooldown; }
        }
       
        float cooldownElapsed;
        public float CooldownElapsed
        {
            get { return cooldownElapsed; }
        }

        PlayerController owner;
        Collider ownerCollider;
        //int activeCoolerCount;
        //float coolerCooldownElapsed;
        float distanceAdjustment;

        float actualDistance;

        float superShotCharge = 0;
        public float SuperShotCharge
        {
            get { return superShotCharge; }
        }
        float superShotChargeReady = 5;
        public float SuperShotChargeReady
        {
            get { return superShotChargeReady; }
        }

        

#if TEST_DISTANCE
        GameObject testSphere;
#endif

        private void Awake()
        {
            ///// ONLY FOR TEST
            ///
            ///////////////////
#if UNITY_EDITOR
            power = TestPlayerStats.WeaponFirePower;
            damage = TestPlayerStats.WeaponDamage;
            fireRate = TestPlayerStats.WeaponFireRate;
            fireRange = TestPlayerStats.WeaponFireRange;
            

#endif

            ///////////////////


            cooldown = 1f / fireRate;
            //activeCoolerCount = coolerCount;



        }

        // Start is called before the first frame update
        void Start()
        {
            if (!owner.photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

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
            //if(activeCoolerCount < coolerCount)
            //{
            //    coolerCooldownElapsed -= Time.deltaTime;
            //    if(coolerCooldownElapsed < 0)
            //    {
            //        activeCoolerCount++;
            //        coolerCooldownElapsed = coolerCooldown;
            //    }
            //}
        }

#if TEST_DISTANCE
        private void LateUpdate()
        {
            TestDistance();
        }

        void TestDistance()
        {
            Vector3 pos = owner.PlayerCamera.transform.position;
            pos += owner.PlayerCamera.transform.forward * (fireRange + owner.PlayerCamera.DistanceAdjustment);
            if(testSphere == null)
            {
                testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(testSphere.GetComponent<Collider>());
            }
                

            testSphere.transform.localScale = Vector3.one * 0.2f;
            testSphere.transform.position = pos;

        }
#endif

        public void IncreaseSuperShotCharge()
        {
            superShotCharge = Mathf.Clamp(superShotCharge, superShotCharge + 1, superShotChargeReady);
        }

        public bool IsSuperShotReady()
        {
            return !(superShotCharge < superShotChargeReady);
        }

        /// <summary>
        /// Local player only.
        /// Returns true if he can shoot and some values are sent back as params 
        /// ( origin, speed, etc ); otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(bool superShot, out object[] parameters)
        {
            //Debug.Log("FireWeapon - TryShoot().");
            parameters = null;

            // Local player only
            if (!owner.photonView.IsMine && !PhotonNetwork.OfflineMode)
                return false;

            // Not ready yet
            if (cooldownElapsed > 0)
                return false;

            // Try super shot
            if (superShot)
            {
                // If not ready don't shoot
                if (!IsSuperShotReady())
                    return false;
                else
                    superShotCharge = 0;// Uncharge the super shot

            }
            


            // Check coolers
            //if (activeCoolerCount == 0)
            //    return false;

            //activeCoolerCount--;

            // Ready to shoot
            cooldownElapsed = cooldown;
            //coolerCooldownElapsed = coolerCooldown;

            // Get origin, direction and speed
            Vector3 origin = owner.PlayerCamera.transform.position;
            Vector3 direction = owner.PlayerCamera.transform.forward;

            // Check for collision
            Ray ray = new Ray(origin, direction);
            RaycastHit info;
            //int mask = ~LayerMask.NameToLayer(Layer.Player);
            ownerCollider.enabled = false;
            bool hit = Physics.Raycast(ray, out info, fireRange + owner.PlayerCamera.DistanceAdjustment);
            ownerCollider.enabled = true;
            if (hit)
            {
                
                IHittable hittable = info.collider.GetComponent<IHittable>();

                if(hittable != null)
                {
                    parameters = new object[6];
                    if (!PhotonNetwork.OfflineMode)
                    {
                        
                        //parameters[0] = info.collider.GetComponent<Ball>().photonView.ViewID;
                        parameters[0] = (hittable as MonoBehaviourPun).photonView.ViewID;
                    }
                    else
                    {
                        parameters[0] = (hittable as MonoBehaviourPun).GetInstanceID();
                        //parameters[0] = (hittable as MonoBehaviourPun);
                    }
                    parameters[1] = info.point;
                    parameters[2] = info.normal;
                    parameters[3] = PhotonNetwork.Time;
                    parameters[4] = direction;
                    parameters[5] = superShot;

                    // If the player is in the enemy goal area then load the super shot
                    if(!superShot && !owner.IsInGoalArea())
                    {
                        //superShotCharge = Mathf.Clamp(superShotCharge, superShotCharge + 1, superShotChargeReady);
                        IncreaseSuperShotCharge();
                    }
                    
                }
            }
            
            return true;
        }




        public void SetOwner(PlayerController owner)
        {
            this.owner = owner;
            ownerCollider = owner.GetComponent<Collider>();
        }

        /// <summary>
        /// Called on every client
        /// </summary>
        /// <param name="parameters"></param>
        public void Shoot(object[] parameters)
        {
            StartCoroutine(ShootDelayed(parameters));
        }


#region private



        IEnumerator ShootDelayed(object[] parameters)
        {
            //Debug.LogFormat("FireWeapon - Shooting: params.Length:{0}", parameters.Length);

            // We can add some fx here
            shootParticle.Play();

            // Audio
            audioSource.Play();

            if (parameters == null)
            {
                yield break;
            }


            // Get params
            GameObject hitObject = null;

            if (!PhotonNetwork.OfflineMode)
            {
                hitObject = PhotonNetwork.GetPhotonView((int)parameters[0]).gameObject;
            }
            else
            {
                hitObject = new List<MonoBehaviourPun>(GameObject.FindObjectsOfType<MonoBehaviourPun>()).Find(m => m.GetInstanceID() == (int)parameters[0]).gameObject;
            }
            Vector3 hitPoint = (Vector3)parameters[1];
            Vector3 hitNormal = (Vector3)parameters[2];
            double timestamp = (double)parameters[3];
            Vector3 hitDirection = (Vector3)parameters[4];
            bool superShot = (bool)parameters[5];

            // Check the time passed
            float lag = (float)(PhotonNetwork.Time - timestamp);
            if (shootDelay >= lag)
                yield return new WaitForSeconds(shootDelay - lag);
            //else
            //    yield break; // It took to much time

            // Shoot
            if (hitObject != null)
            {

                IHittable hittable = hitObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    bool useDamage = false;
                    if (!Tag.Ball.Equals((hittable as MonoBehaviour).tag))
                        useDamage = true;

                    float mul = superShot ? 3f : 1f;

                    hittable.Hit(owner.gameObject, hitPoint, hitNormal, hitDirection, useDamage ? damage * mul : power * mul);
                }
            }
           
        }


#endregion
    }

}
