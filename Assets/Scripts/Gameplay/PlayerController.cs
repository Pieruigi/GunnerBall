#define PEER_SHOT
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zoca.Interfaces;

namespace Zoca
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IHittable
    {
        public static GameObject LocalPlayer { get; private set; }

        public static PlayerController Local { get; private set; }

        [Header("Physics")]
        [SerializeField]
        float maxSpeed = 5f;

        [SerializeField]
        float acceleration = 10f;

        [SerializeField]
        float sprintMultiplier = 2;

        [SerializeField]
        float jumpSpeed = 15f;

        [SerializeField]
        float flyingMultiplier = 0.2f;

        CharacterController cc;

        bool moving = false;
        bool shooting = false;
        Vector2 input;
        Vector2 lookInput;
        float lookSensitivity = 50f;
        public float LookSensitivity
        {
            get { return lookSensitivity; }
        }
      
        Vector3 velocity; // The current character controller velocity
        Vector3 targetVelocity; // The target velocity ( dir * MAX_VEL )
        Vector3 networkPosition; // Position received from this controller's owner
        Quaternion networkRotation; // Position received from this controller's owner
        float lerpSpeed = 10f; // Interpolation speed to adjust network transform

        

        bool jumping = false;
        float ySpeed = 0;
        bool sprinting = false;
        float sprintSpeed;
        float jumpStamina = 20;
        

        // Freezing system
        bool freezed = false;
        DateTime freezedLast;
        bool startPaused = false;
        public bool StartPaused
        {
            get { return startPaused; }
            set { startPaused = value; }
        }

        bool goalPaused = false;
        public bool GoalPaused
        {
            get { return goalPaused; }
            set { goalPaused = value; }
        }

        [SerializeField]
        float freezingCooldown = 4;
        float currentFreezingCooldown;

        [SerializeField]
        float healthMax = 150;
        public float HealthMax
        {
            get { return healthMax; }
        }

        [SerializeField]
        float health = 100;
        public float Health
        {
            get { return health; }
        }

        float healthDefault;

        [SerializeField]
        float stamina = 100; // Used for sprint and jump
        public float Stamina
        {
            get { return stamina; }
        }

        float staminaDefault;
        public float StaminaMax
        {
            get { return staminaDefault; }
        }

        float staminaRechargeDelay = 2;
        float staminaRechargeSpeed = 30;
        float staminaChargeSpeed = 20;
        DateTime staminaLast;

        [Header("Camera")]
        [SerializeField]
        PlayerCamera playerCamera;
        public PlayerCamera PlayerCamera
        {
            get { return playerCamera; }
        }

        [Header("Equipment")]
        [SerializeField]
        FireWeapon fireWeapon;
        public FireWeapon FireWeapon
        {
            get { return fireWeapon; }
        }


        Vector3 startPosition;
        Quaternion startRotation;

        private void Awake()
        {
            // Get the character controller
            cc = GetComponent<CharacterController>();

            // Set the fireweapon owner
            fireWeapon.SetOwner(this);



            healthDefault = health;
            sprintSpeed = maxSpeed * sprintMultiplier;
            staminaDefault = stamina;
            
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            {
                // This is not the local player
                Destroy(playerCamera.gameObject);

                
            }
            else
            {
                // This is the local player
                LocalPlayer = gameObject;

                Local = this;

                // Init player camera
                playerCamera.SetPlayerController(this);

                // Store initial position
                startPosition = transform.position;
                startRotation = transform.rotation;

                // Avoid to destroy the player 
                DontDestroyOnLoad(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;


        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                

                //
                // Check movement
                //
                // Look around
                Vector2 lookAngles = lookInput * lookSensitivity * Time.deltaTime;

                if (freezed)
                {
                    lookAngles = Vector3.zero;
                }

                // Set yaw
                transform.eulerAngles += Vector3.up * lookAngles.x;
                // Set camera pitch
                playerCamera.Pitch(-lookAngles.y);

                // Move character controller
                // Get direction along player forward axis
                Vector3 dir = transform.forward * input.y + transform.right * input.x;
                // Target velocity is the max velocity we can reach
                float speed = maxSpeed;
                //if (!cc.isGrounded)
                //{
                //    speed *= flyingMultiplier;
                //}
                //else
                //{
                    if (sprinting && stamina > 0)
                        speed *= sprintMultiplier;
                //}
                //targetVelocity = dir.normalized * ((sprinting && stamina > 0) ? sprintSpeed : maxSpeed);
                targetVelocity = dir.normalized * speed;
                CheckStamina();

                // Stop moving if paused 
                if (freezed || startPaused)
                {
                    velocity = Vector3.zero;
                    targetVelocity = Vector3.zero;
                }

                // The current velocity takes into account some acceleration
                velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration);
                


                if (freezed || startPaused/* || jumping*/)
                { 
                    ySpeed = 0; 
                }

                // Gravity
                if (!cc.isGrounded)
                {
                    ySpeed += Physics.gravity.y * Time.deltaTime;
                    //Debug.LogFormat("PlayerController - Not grounded; ySpeed; {0}", ySpeed);
                }
                else
                {
                    // When player hits the ground we must reset vertical speed
                    if (ySpeed < 0)
                        ySpeed = 0;

                    jumping = false;
                    //Debug.LogFormat("PlayerController - Grounded; ySpeed; {0}", ySpeed);
                }

                // Set the vertical speed
                velocity.y = ySpeed;

                // Move the character controller
                if(velocity != Vector3.zero)
                {
                    //Debug.LogFormat("PlayerController - Velocity.Y:{0}", velocity.y);
                    cc.Move(velocity * Time.deltaTime);
                }
                    

                //
                // Check shooting
                //

                if (shooting)
                {
                    if (!startPaused && !freezed && !goalPaused)
                    {

                        object[] parameters;
                        //Collider hitCollider;
                        // Returns true if the weapon is ready to shoot, otherwise returns false
                        if (fireWeapon.TryShoot(out parameters))
                        {
                            if (parameters != null)
                            {
                                Debug.LogFormat("PlayerController - Shoot parameters length: {0}", parameters.Length);
                                for (int i = 0; i < parameters.Length; i++)
                                    Debug.LogFormat("PlayerController - Shoot parameter[{0}]: {1}", i, parameters[i]);
                            }


                            // Call rpc on all the clients, even the local one.
                            // By calling it via server we can balance lag.
                            if (!PhotonNetwork.OfflineMode)
                            {
                                photonView.RPC("RpcShoot", RpcTarget.AllViaServer, parameters as object);
                            }
                            else
                            {
                                // In offline mode we call the weapon.Shoot() directly
                                fireWeapon.Shoot(parameters);
                            }
                        }
                    }

 
                }

                // Check if the player has been freezed and eventually recover it
                if (freezed)
                {
                    currentFreezingCooldown -= Time.deltaTime;

                    if (currentFreezingCooldown > 0)
                        return;

                    // Heal
                    freezed = false;
                    health = healthDefault;
                    freezedLast = DateTime.UtcNow;
                }
                else
                {
                    if (health == 0)
                    {
                        Debug.LogFormat("PlayerController - Health is empty, freezing player...");
                        currentFreezingCooldown = freezingCooldown;
                        freezed = true;
                
                    }
                }
            }
            else
            {
                // Remote player, lerp networked position
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * lerpSpeed);
            }

        }

        public void ResetPlayer()
        {
            if (photonView.IsMine)
            {
                velocity = Vector3.zero; 
                targetVelocity = Vector3.zero;
                
                cc.Move(startPosition - transform.position);
                transform.rotation = startRotation;

            }
        }

        public void Hit(GameObject owner, Vector3 hitPoint, Vector3 hitNormal, float hitDamage)
        {
            
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode) // Remote players
            {
                // Stop remote players and eventually apply some fx
                networkPosition = transform.position;
                networkRotation = transform.rotation;
                
            }
            else // Local player
            {
                
                if (freezed || goalPaused || startPaused)
                    return;

                // You get invincibility for a while after recovering from freezing
                if ((DateTime.UtcNow - freezedLast).TotalSeconds < 1)
                    return;

                health = Mathf.Max(0, health - hitDamage);
            }
            
           
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting) // Local player
            {

                stream.SendNext(PhotonNetwork.Time);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(velocity);
                stream.SendNext((byte)health);
            }
            else // Remote player
            {

                double time = (double)stream.ReceiveNext();
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                velocity = (Vector3)stream.ReceiveNext();
                health = (byte)stream.ReceiveNext();

                // Taking lag into account
                float lag = (float)(PhotonNetwork.Time - time);
                networkPosition += velocity * lag;



            }
        }

        #region input_system_callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

           
            if (context.performed)
            {
                Debug.LogFormat("PlayerController - Moving...................");
                if (!moving)
                {
                    moving = true;
                    
                    // Start moving
                    //animator.SetBool(walkParam, true);

                    //Debug.LogFormat("Player starts moving");
                }

                input = context.ReadValue<Vector2>();
                
            }
            else
            {
                if (moving)
                {
                    moving = false;

                    // Stop moving
                    //animator.SetBool(walkParam, false);

                    //Debug.LogFormat("Player stops moving");
                }

                input = Vector2.zero;
            }

        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

           
            lookInput = context.ReadValue<Vector2>();
                
            //Debug.LogFormat("PlayerController - Look input: {0}", lookInput);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            return;

            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (stamina < jumpStamina)
                return;

            if (jumping)
                return;

            // Jump
            if (cc.isGrounded)
            {
                jumping = true;
                stamina -= jumpStamina;
                staminaLast = DateTime.UtcNow;
                ySpeed = jumpSpeed;
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (context.performed)
            {
                if (!sprinting)
                {
                    sprinting = true;
                }
            }
            else
            {
                if (sprinting)
                {
                    sprinting = false;
                }
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

           
            if (context.performed)
            {
                if (!shooting)
                {
                    shooting = true;
                }
            }
            else
            {
                if (shooting)
                {
                    shooting = false;
                }
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (context.started)
                GameManager.Instance.Pause();
        }


        #endregion

        #region private
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        //        return;

        //    if (Tag.Ball.Equals(collision.transform.tag))
        //    {
        //        // Apply a lot of damage
        //        Hit(collision.gameObject, Vector3.zero, Vector3.zero, 1000);
        //    }
        //}
        
        void CheckStamina()
        {
            if (sprinting)
            {
                if(stamina > 0)
                    stamina = Mathf.Max(0, stamina - Time.deltaTime * staminaChargeSpeed);

                staminaLast = DateTime.UtcNow;
            }
            else
            {
                if (stamina < staminaDefault)
                {
                    // Some delay before reloading stamina
                    if((DateTime.UtcNow - staminaLast).TotalSeconds > staminaRechargeDelay)
                        stamina = Mathf.Min(staminaDefault, stamina + Time.deltaTime * staminaRechargeSpeed);
                }
                    
            }
        }

        #endregion


        #region rpc

        [PunRPC]
        void RpcShoot(object[] parameters, PhotonMessageInfo info)
        {
            if (parameters != null)
            {
                Debug.LogFormat("PlayerController - RpcShoot parameters count: {0}", parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                    Debug.LogFormat("PlayerController - RpcShoot parameter[{0}]: {1}", i, parameters[i]);
            }


            fireWeapon.Shoot(parameters);
        }

        #endregion

    }

}
