//#define PEER_SHOT -- NO LONGER USED
//#define SYNC_MOVE_INPUT
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
        public float MaxSpeed
        {
            get { return maxSpeed; }
        }

        [SerializeField]
        float acceleration = 10f;

        [SerializeField]
        float sprintMultiplier = 2;
        public float SprintMultiplier
        {
            get { return sprintMultiplier; }
        }

        [SerializeField]
        float jumpSpeed = 15f;

        [SerializeField]
        float flyingMultiplier = 0.2f;

        CharacterController cc;

        bool moving = false;
        
        bool shooting = false;
        Vector2 moveInput;
        public Vector2 MovementInput
        {
            get { return moveInput; }
        }

        Vector2 lookInput;
        float lookSensitivity = 50f;
        public float LookSensitivity
        {
            get { return lookSensitivity; }
        }
      
        Vector3 velocity; // The current character controller velocity
        public Vector3 Velocity
        {
            get { return velocity; }
        }
        Vector3 targetVelocity; // The target velocity ( dir * MAX_VEL )
        Vector3 networkPosition; // Position received from this controller's owner
        Quaternion networkRotation; // Position received from this controller's owner
        float lerpSpeed = 10f; // Interpolation speed to adjust network transform
#if SYNC_MOVE_INPUT
        Vector2 networkMoveInput;
#endif        


        bool jumping = false;
        float ySpeed = 0;
        bool sprinting = false;
        bool sprintInput = false;
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


        float ballPowerOnHit = 20;

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
                
                /**
                 * Look around
                 */
                Vector2 lookAngles = lookInput * lookSensitivity * Time.deltaTime;

                if (freezed)
                {
                    lookAngles = Vector3.zero;
                }

                // Set yaw
                transform.eulerAngles += Vector3.up * lookAngles.x;
                // Set camera pitch
                playerCamera.Pitch(-lookAngles.y);

                /**
                 * Move around
                 */
                // Get the direction along the player forward axis
                Vector3 dir = transform.forward * moveInput.y + transform.right * moveInput.x;
                // Target velocity is the max velocity we can reach
                float speed = maxSpeed;
                //if (!cc.isGrounded)
                //{
                //    speed *= flyingMultiplier;
                //}
                //else
                //{
                // You can only sprint if you are moving straight forward:
                sprinting = sprintInput;
                if (moveInput.y <= 0 || moveInput.x != 0)
                    sprinting = false;

                // Check stamina and try to adjust speed
                if (sprinting && stamina > 0)
                    speed *= sprintMultiplier;
                //}

                // Here we consume and refill stamina
                CheckStamina();

                //targetVelocity = dir.normalized * ((sprinting && stamina > 0) ? sprintSpeed : maxSpeed);
                targetVelocity = dir.normalized * speed;
                

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

#if SYNC_MOVE_INPUT
                // Remote player, lerp input
                moveInput = Vector2.MoveTowards(moveInput, networkMoveInput, Time.deltaTime * 5);
#endif
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
                //// Stop remote players and eventually apply some fx
                //networkPosition = transform.position;
                //networkRotation = transform.rotation;
                
            }
            else // Local player
            {
                
                if (freezed || goalPaused || startPaused)
                    return;

                // You get invincibility for a while after recovering from freezing
                if ((DateTime.UtcNow - freezedLast).TotalSeconds < 1)
                    return;

                health = Mathf.Max(0, health - hitDamage);

                // If the ball hit the player we let the ball bounce away.
                if (Tag.Ball.Equals(owner.tag))
                {
                    Vector3 bounce = owner.transform.position - transform.position;
                    bounce = bounce.normalized * ballPowerOnHit;
                    Debug.LogFormat("PlayerController - Ball bouncing, newVelocity: {0}", bounce);
                    Ball.Instance.photonView.RPC("RpcHitByPlayer", RpcTarget.AllViaServer, bounce, PhotonNetwork.Time);
                }
            }
            
           
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting) // Local player
            {

                stream.SendNext(PhotonNetwork.Time);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(velocity); // Synch for better behaviour
                stream.SendNext((byte)health);

#if SYNC_MOVE_INPUT
                // For animator
                stream.SendNext(moveInput);
#endif
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

#if SYNC_MOVE_INPUT
                // For animator
                networkMoveInput = (Vector2)stream.ReceiveNext();
#endif

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

                moveInput = context.ReadValue<Vector2>();
                
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

                moveInput = Vector2.zero;
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
                if (!sprintInput && moveInput.x == 0 && moveInput.y > 0)
                {
                    //sprinting = true;
                    sprintInput = true;
                }
            }
            else
            {
                if (sprintInput)
                {
                    sprintInput = false;
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

        //public void OnSwitchCamera(InputAction.CallbackContext context)
        //{
        //    if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        //        return;

        //    if (context.started && !playerCamera.IsSwitching())
        //    {
        //        playerCamera.Switch();
        //    }
        //}

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
