using DG.Tweening;
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
        public float Power
        {
            get { return power; }
            set { power = value; }
        }

        float powerDefault;
        public float PowerDefault
        {
            get { return powerDefault; }
        }

        //[SerializeField]
        float damage = 10000;

        [SerializeField]
        float fireRate = 0.2f;
        public float FireRate
        {
            get { return fireRate; }
            set { fireRate = value; cooldown = 1f / fireRate; }
        }

        float fireRateDefault;
        public float FireRateDefault
        {
            get { return fireRateDefault; }
        }

        [SerializeField]
        float fireRange = 8.6f;
        public float FireRange
        {
            get { return fireRange; }
            set { fireRange = value; }
        }

        float fireRangeDefault;
        public float FireRangeDefault
        {
            get { return fireRangeDefault; }
        }

        [SerializeField]
        float fireRadius = 0.5f;
        public float FireRadius
        {
            get { return fireRadius; }
        }

        [Header("Fx")]
        [SerializeField]
        ParticleSystem shootParticle;

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        GameObject trailParticlePrefab;

        [SerializeField]
        AudioSource failedAudioSource;

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

        float failedCooldown = 0.5f;
        
        PlayerController owner;
        Collider ownerCollider;

        float distanceAdjustment;

        float actualDistance;
        float superShotPowerMul = 2;
        float pushBackForceHigh = 15;
        float pushBackForceLow = 5;

        private void Awake()
        {

            cooldown = 1f / fireRate;

            powerDefault = power;
            fireRateDefault = fireRate;
            fireRangeDefault = fireRange;

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
            //Debug.Log("FireW:" + transform.rotation);

            // Recall
            if(cooldownElapsed > 0)
            {
                cooldownElapsed -= Time.deltaTime;
            }

        }

        public bool CheckSuperShot()
        {
            //return false;      
            //Ray ray = new Ray(owner.transform.position, Vector3.down);
            //if (Physics.Raycast(ray, 0.5f))
            //    return false;
            if (owner.IsGrounded())
                return false;

            if (!owner.Jumping)
                return false;

            // Get player vertical velocity
            float verticalSpeed = owner.Velocity.y;
            if (Mathf.Abs(verticalSpeed) > 7)
                return false;

            return true;
        }


        public bool CanShoot()
        {
            return cooldownElapsed <= 0;
        }

        
        /// <summary>
        /// Local player only.
        /// Returns true if he can shoot and some values are sent back as params 
        /// ( origin, speed, etc ); otherwise return false.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool TryShoot(out object[] parameters)
        {
            //Debug.Log("FireWeapon - TryShoot().");
            parameters = null;

            // Local player only
            //if (!owner.photonView.IsMine && !PhotonNetwork.OfflineMode)
            //    return false;

            // Not ready yet
            if (cooldownElapsed > 0)
            {
                //failedAudioSource.Play();
                return false;
            }
                

            // Ready to shoot
            cooldownElapsed = cooldown;
            //coolerCooldownElapsed = coolerCooldown;

            // Get origin, direction and speed
            Vector3 origin = owner.PlayerCamera.transform.position;
            Vector3 direction = owner.PlayerCamera.transform.forward;

            // Check for collision
            Ray ray = new Ray(origin, direction);
            RaycastHit info;

            /************************ Using ray *************************
            ownerCollider.enabled = false;
            Debug.DrawRay(ray.origin, ray.direction * (fireRange + owner.PlayerCamera.DistanceAdjustment), Color.red, 30);
            bool hit = Physics.Raycast(ray, out info, fireRange + owner.PlayerCamera.DistanceAdjustment);
            ownerCollider.enabled = true;
            **************************************************************/

            /************************ Using sphere ************************/
            //float radius = fireRadius;
            //ownerCollider.enabled = false;
            //float maxDistance = fireRange + owner.PlayerCamera.DistanceAdjustment - radius;
            //int layer = LayerMask.GetMask(new string[] { Layer.Ground, Layer.Wall });
            //bool hit = Physics.SphereCast(ray, radius, out info, maxDistance, ~layer);
            //ownerCollider.enabled = true;
            bool hit = HasTarget(out info);
            /**************************************************************/

            bool superShot = CheckSuperShot();
            //if (!owner.GetComponent<CharacterController>().isGrounded)
            //{
                
            //    if (superShot)
            //    {
            //        owner.PushBack(pushBackForceHigh);
            //    }
            //    else
            //    {
            //        owner.PushBack(pushBackForceLow);
            //    }
            //}
            

            Debug.Log("Checking super shot:" + superShot);
            if (hit)
            {
                
                IHittable hittable = info.collider.GetComponent<IHittable>();

                if(hittable != null)
                {
                    List<object> paramList = new List<object>();
                    paramList.Add((hittable as MonoBehaviourPun).photonView.ViewID);
                    paramList.Add(info.point);
                    paramList.Add(info.normal);
                    paramList.Add(PhotonNetwork.Time);
                    paramList.Add(direction);

                    //parameters = new object[5];
                    //parameters[0] = (hittable as MonoBehaviourPun).photonView.ViewID;
                    //parameters[1] = info.point;
                    //parameters[2] = info.normal;
                    //parameters[3] = PhotonNetwork.Time;
                    //parameters[4] = direction;
                    
                    if (Tag.Ball.Equals(info.collider.tag))
                    {
                        // Hit the ball, so lets check for the super shot
                        if (superShot)
                        {
                            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA SuperShot");
                            paramList.Add((byte)1);
                        }
                        else
                        {
                            paramList.Add((byte)0);
                        }    
                    }

                    parameters = paramList.ToArray();

                }
            }
            
            return true;
        }

        public bool HasTarget(out RaycastHit info)
        {
            // Get origin, direction and speed
            Vector3 origin = owner.PlayerCamera.transform.position;
            Vector3 direction = owner.PlayerCamera.transform.forward;

            // Check for collision
            Ray ray = new Ray(origin, direction);

            float radius = fireRadius;
            ownerCollider.enabled = false;
            float maxDistance = fireRange + owner.PlayerCamera.DistanceAdjustment - radius;
            int layer = LayerMask.GetMask(new string[] { Layer.Ground, Layer.Wall });
            bool hit = Physics.SphereCast(ray, radius, out info, maxDistance, ~layer);
            ownerCollider.enabled = true;
            return hit;
        }

        public bool TryShootPowerUp(SpecialSkillPowerUp powerUp)
        {
            if (cooldownElapsed > 0)
            {
                //failedAudioSource.Play();
                return false;
            }
                

            if (!powerUp || !powerUp.CanShoot())
            {
                failedAudioSource.Play();
                return false;
            }
                

            bool ret = powerUp.TryShoot();

            if (!ret)
            {
                if (!(cooldownElapsed > 0))
                    cooldownElapsed = failedCooldown;

                failedAudioSource.Play();
            }
            else
            {
                cooldownElapsed = cooldown;
            }

            return ret;

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

        public void ShootPowerUp(object[] parameters)
        {
            shootParticle.Play();

            // Audio
            audioSource.Play();

            // Create trail
            Instantiate(trailParticlePrefab, shootParticle.transform.position, transform.rotation, transform);
            
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

            //if (!PhotonNetwork.OfflineMode)
            //{
                hitObject = PhotonNetwork.GetPhotonView((int)parameters[0]).gameObject;
            //}
            //else
            //{
            //    hitObject = new List<MonoBehaviourPun>(GameObject.FindObjectsOfType<MonoBehaviourPun>()).Find(m => m.GetInstanceID() == (int)parameters[0]).gameObject;
            //}
            Vector3 hitPoint = (Vector3)parameters[1];
            Vector3 hitNormal = (Vector3)parameters[2];
            double timestamp = (double)parameters[3];
            Vector3 hitDirection = (Vector3)parameters[4];
            //bool superShot = (bool)parameters[5];

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
                    bool superShot = false;
                    if (!Tag.Ball.Equals((hittable as MonoBehaviour).tag))
                    {
                        useDamage = true;
                    }
                    else
                    {
                        // Ball hit, lets check the sixth parameter
                        superShot = (byte)parameters[5] == 1 ? true : false;

                        if (superShot)
                            hitObject.GetComponent<Ball>().ActivateSuperShotFx();
                    }
                   
                    hittable.Hit(owner.gameObject, hitPoint, hitNormal, hitDirection, useDamage ? damage : !superShot ? power : superShotPowerMul * power);

                    
                }
            }
           
        }


#endregion
    }

}
