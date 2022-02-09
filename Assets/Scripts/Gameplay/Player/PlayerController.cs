//#define PEER_SHOT -- NO LONGER USED
//#define SYNC_MOVE_INPUT
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zoca.AI;
using Zoca.Collections;
using Zoca.Interfaces;

namespace Zoca
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable//, IHittable
    {
        //public UnityAction OnLeaveRoomRequest;
        public UnityAction OnPaused;

        public static GameObject LocalPlayer { get; private set; }

        public static PlayerController Local { get; private set; }

        [Header("Physics")]
        #region movement
        [SerializeField]
        float maxSpeed = 5f;
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
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
        float jumpSpeed = 75f;

        [SerializeField]
        float flyingMultiplier = 0.2f;

        CharacterController cc;

        bool moving = false;

        Vector2 moveInput;
        public Vector2 MovementInput
        {
            get { return moveInput; }
        }

        Vector3 velocity; // The current character controller velocity
        public Vector3 Velocity
        {
            get { return velocity; }
        }
        Vector3 targetVelocity; // The target velocity ( dir * MAX_VEL )

        float lerpSpeed = 10f; // Interpolation speed to adjust network transform

        bool jumping = false;
        public bool Jumping
        {
            get { return jumping; }
        }

        float ySpeed = 0;
        bool sprinting = false;
        public bool Sprinting
        {
            get { return sprinting; }
        }
        bool sprintInput = false;
        float sprintSpeed;
        float jumpStamina = 20;

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
            set { staminaDefault = value; }
        }


        float staminaRechargeDelay = 2;
        float staminaRechargeSpeed = 30;
        float staminaChargeSpeed = 20;
       
        DateTime staminaLast;

        #endregion

       

        #region look
        Vector2 lookInput;
        float lookSensitivityMul = 2.5f;
        float yawSpeed = 720;
        float yawSpeedOnSprint = 360;
        float pitchSpeed = 240;
        
        //public float LookSensitivity
        //{
        //    get { return lookSensitivity; }
        //}

        float maxPitch = 30;
        float minPitch = -70;

        float currentPitch;
        public float CurrentPitch
        {
            get { return currentPitch; }
        }

        #endregion

        #region manager
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
        #endregion

        #region fight
        //[SerializeField]
        public float FreezingCooldown
        {
            get { return freezingCooldown; }
            set { freezingCooldown = value; }
        }
        [SerializeField]
        float freezingCooldown = 4;

        float currentFreezingCooldown;
        bool freezed = false;
        DateTime freezedLast;

        bool shooting = false;
        bool superShoot = false;

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

        #endregion

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

        //[SerializeField]
        //FireWeapon[] weapons;

        [SerializeField]
        Transform[] weaponPivots; // The pivot X holds the gun X

        [Header("Fx")]
        [SerializeField]
        ParticleSystem freezeParticle;

        [SerializeField]
        ParticleSystem goalAreaBlueParticle;

        [SerializeField]
        ParticleSystem goalAreaRedParticle;

        [SerializeField]
        AudioSource freezeAudioSource;


        #region misc
        float ballPowerOnHit = 2.8f;

        Vector3 startPosition;
        Quaternion startRotation;

        bool inGoalArea = false;
       
        #endregion

        #region networked_fields
        float networkPitch;
        Vector3 networkPosition; // Position received from this controller's owner
        Quaternion networkRotation; // Position received from this controller's owner
#if SYNC_MOVE_INPUT
        Vector2 networkMoveInput;
#endif
        #endregion

        AnimationController animationController;

        private void Awake()
        {
            // Get the character controller
            cc = GetComponent<CharacterController>();

            SetCurrentFireWeapon();

            healthDefault = health;
            sprintSpeed = maxSpeed * sprintMultiplier;
            staminaDefault = stamina;

            animationController = GetComponent<AnimationController>();

            // Init player camera
            // Camera is also needed by the aiming system for ais
            playerCamera.SetPlayerController(this);

            // Store initial position
            startPosition = transform.position;
            startRotation = transform.rotation;

            Debug.LogFormat("PlayerController - playerName: {0}, isMine: {1}", gameObject.name, photonView.IsMine);

            if (photonView.IsMine && !photonView.IsRoomView)
            {
                Debug.LogFormat("PlayerController - setting mine: {0}", gameObject.name);

                ///// ONLY FOR TEST
                ///
                ///////////////////
//#if UNITY_EDITOR
//                maxSpeed = TestPlayerStats.PlayerMaxSpeed;
//                sprintMultiplier = TestPlayerStats.PlayerSprintMultiplier;
//                health = TestPlayerStats.PlayerHealth;
//                healthMax = TestPlayerStats.PlayerHealthMax;
//                freezingCooldown = TestPlayerStats.PlayerFreezingTime;

//#endif

                ///////////////////


                // This is the local player
                LocalPlayer = gameObject;

                Local = this;

                // Avoid to destroy the player 
                DontDestroyOnLoad(gameObject);



            }

           
        }

        // Start is called before the first frame update
        

        // Update is called once per frame
        void Update()
        {
            

            if (photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                

                // Check for sprinting input
                if (sprintInput && stamina > 0)
                {
                    sprinting = true;
                }
                else
                {
                    sprinting = false;
                }

                /**
                 * Look around
                 */
                Vector2 lookAngles = lookInput * SettingsManager.Instance.MouseSensitivity * lookSensitivityMul * Time.deltaTime;

                if (freezed)
                {
                    lookAngles = Vector3.zero;
                }

                
                //if (sprinting)
                //{
                //    // Reduce the yaw 
                //    lookAngles.x *= 0.05f;
                //}

              
                // Set yaw
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, transform.eulerAngles + Vector3.up * lookAngles.x, Time.deltaTime * (sprinting ? yawSpeedOnSprint : yawSpeed));

                // Set camera pitch
                currentPitch = Mathf.MoveTowards(currentPitch, currentPitch - lookAngles.y, Time.deltaTime * pitchSpeed);
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
                playerCamera.SetPitch(currentPitch);

                /**
                 * Move around
                 */
                
                // If the player is sprinting we remove the x input and set y = 1
                Vector3 dir = Vector3.zero;
                float speed = maxSpeed;
                if (sprinting)
                {
                    // You can only sprint straight forward
                    dir = transform.forward;
                    speed *= sprintMultiplier;
                }
                else
                {
                    // You can move along z and x axis
                   
                    dir = transform.forward * moveInput.y + transform.right * moveInput.x;
                    //dir = new Vector3(moveInput.x, 0, moveInput.y);
                   
                }
                
                // Target velocity is the max velocity we can reach
                
                //if (!cc.isGrounded)
                //{
                //    speed *= flyingMultiplier;
                //}
                //else
                //{
                    
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
                float accMul = 1;
                if (!cc.isGrounded)
                    accMul *= 0.2f;
                

                velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration * accMul);


                if (freezed || startPaused/* || jumping*/)
                { 
                    ySpeed = 0; 
                }


                //// Gravity
                //if (!cc.isGrounded)
                //{
                //    ySpeed += Physics.gravity.y * Time.deltaTime;
                //    //Debug.LogFormat("PlayerController - Not grounded; ySpeed; {0}", ySpeed);
                //}
                //else
                //{
                //    // When player hits the ground we must reset vertical speed
                //    if (ySpeed < 0)
                //        ySpeed = 0;

                //    jumping = false;
                //    //Debug.LogFormat("PlayerController - Grounded; ySpeed; {0}", ySpeed);
                //}

                // Set the vertical speed
                velocity.y = ySpeed;

                // Move the character controller
                if(velocity != Vector3.zero)
                {
                    //Debug.LogFormat("PlayerController - Velocity.Y:{0}", velocity.y);
                    cc.Move(velocity * Time.deltaTime);
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


                //
                // Check shooting
                //

                if (shooting)
                {
                    //Debug.Log("Update - shooting...");
                    if (!startPaused && !freezed && !goalPaused)
                    {
                        //Debug.Log("Update - shooting 2...");
                        object[] parameters;
                     
                        // Returns true if the weapon is ready to shoot, otherwise returns false
                        if (fireWeapon.TryShoot(superShoot, out parameters))
                        {

                            // Call rpc on all the clients, even the local one.
                            // By calling it via server we can balance lag.
                            photonView.RPC("RpcShoot", RpcTarget.AllViaServer, parameters as object);
                            //if (!PhotonNetwork.OfflineMode)
                            //{
                            //    photonView.RPC("RpcShoot", RpcTarget.AllViaServer, parameters as object);
                            //}
                            //else
                            //{
                               
                            //    photonView.RPC("RpcShoot", RpcTarget.AllViaServer, parameters as object);
                            //}

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
                        //Debug.LogFormat("PlayerController - Health is empty, freezing player...");
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
                currentPitch = Mathf.Lerp(currentPitch, networkPitch, Time.deltaTime * lerpSpeed);
                //transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * lerpSpeed * 5f);
                //transform.rotation =  Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * lerpSpeed * 60f);
                //currentPitch = Mathf.MoveTowards(currentPitch, networkPitch, 360 * Time.deltaTime);
#if SYNC_MOVE_INPUT
                // Remote player, lerp input
                moveInput = Vector2.MoveTowards(moveInput, networkMoveInput, Time.deltaTime * 5);
#endif
            }

        }

        private void LateUpdate()
        {
            // Run this for both local and remote players
            if (freezed)
            {
#if !FX_DISABLED
                if (!freezeParticle.isPlaying)
                    freezeParticle.Play();
#endif

                if (!freezeAudioSource.isPlaying)
                    freezeAudioSource.Play();


                animationController.AnimateFreeze(true);
            }
            else
            {
#if !FX_DISABLED
                if (freezeParticle.isPlaying)
                    freezeParticle.Stop();
#endif

                if (freezeAudioSource.isPlaying)
                    freezeAudioSource.Stop();

                animationController.AnimateFreeze(false);
            }
        }

       

        public void LookAt(Vector3 target)
        {
         
            // Rotate towards the target
            Vector3 targetFwd = target - transform.position;
            //transform.forward = Vector3.MoveTowards(transform.forward, new Vector3(targetFwd.x, 0, targetFwd.z), Time.deltaTime * (sprinting ? yawSpeedOnSprint : yawSpeed));
            transform.forward = new Vector3(targetFwd.x, 0, targetFwd.z);
            // Add pitch
            Vector3 cameraToTarget = target - playerCamera.transform.position;
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0; // Reset the pitch
            Vector3 planeNormal = playerCamera.transform.right;
            Vector3 cameraToTargetProj = Vector3.ProjectOnPlane(cameraToTarget, planeNormal);
            
            float angle = Vector3.SignedAngle(cameraForward, cameraToTargetProj, planeNormal);
            
            currentPitch = angle;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        public bool IsInGoalArea()
        {
            return inGoalArea;
        }

        public void EnterGoalArea(GoalArea goalArea)
        {
            
            if (inGoalArea)
                return;
            
            if(goalArea.Team == (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(photonView.Owner, PlayerCustomPropertyKey.TeamColor))
            {
                // This player is in its goal area
                inGoalArea = true;
                

            }

        }

        public void ExitGoalArea(GoalArea goalArea)
        {
            if (!inGoalArea)
                return;
            if (goalArea.Team == (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(photonView.Owner, PlayerCustomPropertyKey.TeamColor))
            {
                inGoalArea = false;
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

                inGoalArea = false;
            }
        }

        public void Move(bool move, Vector2? direction = null)
        {
            moving = move;

            if (move)
            {
             
                Vector2 t = new Vector2(transform.right.x, transform.right.z);
                float dotX = Vector2.Dot(direction.Value, t);
                t = new Vector2(transform.forward.x, transform.forward.z);
                float dotZ = Vector2.Dot(direction.Value, t);

                moveInput = new Vector2(dotX, dotZ).normalized;
            }
            else
            {
                moveInput = Vector2.zero;
            }

        }

        public void Shoot(bool value)
        {
           
            // Always set the super shoot false
            superShoot = false;

            // You can't shoot if you are sprinting
            if (sprinting)
            {
                shooting = false;

                return;
            }

            
            if (value)
            {

                shooting = true;

            }
            else
            {
                shooting = false;

            }
        }

        public void Sprint(bool value)
        {
            sprintInput = value;
            if (!value)
                sprinting = value;
        }

        public void Hit(GameObject owner, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection, float hitDamage)
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
                    //Debug.LogFormat("PlayerController - Ball bouncing, newVelocity: {0}", bounce);
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
                stream.SendNext(currentPitch);

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
                networkPitch = (float)stream.ReceiveNext();
                // Taking lag into account
                float lag = (float)(PhotonNetwork.Time - time);
                networkPosition += velocity * lag;

                // Freeze
                if (health == 0)
                    freezed = true;
                else
                    freezed = false;

#if SYNC_MOVE_INPUT
                // For animator
                networkMoveInput = (Vector2)stream.ReceiveNext();
#endif

            }
        }

#region input_system_callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;

                   
            if (context.performed)
            {
                 
                moving = true;
                
                
                moveInput = context.ReadValue<Vector2>();
                
            }
            else
            {
                moving = false;
                
                moveInput = Vector2.zero;
                
            }

        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;

           
            lookInput = context.ReadValue<Vector2>();
                
            //Debug.LogFormat("PlayerController - Look input: {0}", lookInput);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            //return;

            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
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
            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;

            if (context.performed)
            {
                Sprint(true);
          
            }
            else
            {
                Sprint(false);
               
            }
        }

      

        public void OnShoot(InputAction.CallbackContext context)
        {
         

            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;


            Shoot(context.performed);

            
        }

        public void OnSuperShoot(InputAction.CallbackContext context)
        {
            return;

            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;

            // Always set the super shoot false
            superShoot = false;

            // You can't shoot if you are sprinting
            if (sprinting)
            {
                shooting = false;
                return;
            }


            if (context.performed)
            {

                if (!shooting)
                {
                    shooting = true;
                    superShoot = true;
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
            if (!photonView.IsMine || (PhotonNetwork.OfflineMode && photonView.Owner != PhotonNetwork.MasterClient))
                return;

            if (context.started)
                OnPaused?.Invoke();
        }

       


#endregion

#region private
        private void OnDestroy()
        {
            //Destroy(playerCamera.gameObject);
        }

        void SetCurrentFireWeapon()
        {
            //// Set all weapons off
            //foreach(FireWeapon weapon in weapons)
            //{
            //    weapon.gameObject.SetActive(false);
            //}

            // Get the fireweapon id from the player properties
            int  weaponId = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(photonView.Owner, PlayerCustomPropertyKey.WeaponId);
            
            // Get the character id
            int characterId = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(photonView.Owner, PlayerCustomPropertyKey.CharacterId);

            Debug.Log("CharacterId:" + characterId);
            // Load character from collection
            Character character = new List<Character>(Resources.LoadAll<Character>(Character.CollectionFolder)).Find(c => c.name.StartsWith(string.Format("{0}.", characterId+1)));

            Debug.Log("Found character:" + character);

            // Get the prefab
            GameObject weaponPrefab = character.Weapons[weaponId].GameAsset;

            // Create the asset
            GameObject weaponObject = GameObject.Instantiate(weaponPrefab);

            // Set the pivot as parent
            weaponObject.transform.parent = weaponPivots[weaponId];
            // Reset coordinates
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.identity;

            // Set the weapon
            fireWeapon = weaponObject.GetComponent<FireWeapon>();
            // Activate the current weapon
            fireWeapon.gameObject.SetActive(true);
            // Set the fireweapon owner
            fireWeapon.SetOwner(this);

            //// Set the weapon
            //fireWeapon = weapons[weaponId];
            //// Activate the current weapon
            //fireWeapon.gameObject.SetActive(true);
            //// Set the fireweapon owner
            //fireWeapon.SetOwner(this);
        }


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
           
            animationController.AnimateShoot(1f);

            fireWeapon.Shoot(parameters);
        }

        

#endregion

    }

}
