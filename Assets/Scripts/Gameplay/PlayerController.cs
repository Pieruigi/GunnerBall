#define PEER_SHOT
using Photon.Pun;
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
        Collider playerCollider;

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
        //float lookSpeed = 60f;
        Vector3 velocity; // The current character controller velocity
        Vector3 targetVelocity; // The target velocity ( dir * MAX_VEL )
        Vector3 networkPosition; // Position received from this controller's owner
        Quaternion networkRotation; // Position received from this controller's owner
        float lerpSpeed = 10f; // Interpolation speed to adjust network transform

        float jumpSpeed = 3f;
        bool jumping = false;
        float ySpeed = 0;

        bool moveDisabled = false;
        public bool MoveDisabled
        {
            get { return moveDisabled; }
            set 
            { 
                moveDisabled = value; 
                if (value) 
                { 
                    moving = false; 
                    input = Vector2.zero; 
                } 
            }
        }
        bool lookDisabled = false;
        public bool LookDisabled
        {
            get { return lookDisabled; }
            set { lookDisabled = value; }
        }
        bool shootDisabled = false;
        public bool ShootDisabled
        {
            get { return shootDisabled; }
            set { shootDisabled = value; if (value) shooting = false; }
        }

        [SerializeField]
        GameObject damageable;


        GameObject damageableDefault;

        Vector3 startPosition;
        Quaternion startRotation;

        private void Awake()
        {
            // Get the character controller
            cc = GetComponent<CharacterController>();

            // Set the fireweapon owner
            fireWeapon.SetOwner(this);

            // Store default health damageable
            damageableDefault = damageable;
            
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
                // Set yaw
                transform.eulerAngles += Vector3.up * lookAngles.x;
                // Set camera pitch
                playerCamera.Pitch(-lookAngles.y);

                // Move character controller
                // Get direction along player forward axis
                Vector3 dir = transform.forward * input.y + transform.right * input.x;
                // Target velocity is the max velocity we can reach
                targetVelocity = dir.normalized * maxSpeed;
                // The current velocity takes into account some acceleration
                velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration);

                // Are jou jumping?
                if (jumping)
                {
                    jumping = false;
                    ySpeed = jumpSpeed;
                }

                // Gravity
                if (!cc.isGrounded)
                {
                    ySpeed += Physics.gravity.y * Time.deltaTime;
                }
                else
                {
                    // When player hit the ground we must reset vertical speed if any
                    if (ySpeed < 0)
                        ySpeed = 0;
                }

                // Set the vertical speed
                velocity.y = ySpeed;

                // Move the character controller
                cc.Move(velocity * Time.deltaTime);

                //
                // Check shooting
                //

                if (shooting)
                {
                    object[] parameters;
                    //Collider hitCollider;
                    // Returns true if the weapon is ready to shoot, otherwise returns false
                    if (fireWeapon.TryShoot(out parameters))
                    {
                        if(parameters != null)
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

#region input_system_callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (moveDisabled)
                return;
                

            if (context.performed)
            {
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

            if (lookDisabled)
                return;

            lookInput = context.ReadValue<Vector2>();
                
            //Debug.LogFormat("PlayerController - Look input: {0}", lookInput);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (moveDisabled)
                return;

            // Jump
            if (cc.isGrounded)
            {
                jumping = true;
                
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (shootDisabled)
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

        #region private


        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting) // Local player
            {

                stream.SendNext(PhotonNetwork.Time);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(velocity);

            }
            else // Remote player
            {

                double time = (double)stream.ReceiveNext();
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                velocity = (Vector3)stream.ReceiveNext();

                // Taking lag into account
                float lag = (float)(PhotonNetwork.Time - time);
                networkPosition += velocity * lag;

            }
        }

        public void Hit(GameObject owner, Vector3 hitPoint, Vector3 hitNormal, float hitDamage)
        {
            // Apply damage to the current damageable
            IDamageable iDamageable = damageable.GetComponent<IDamageable>();
            iDamageable.ApplyDamage(hitDamage);

            if (iDamageable.IsDestroyed())
            {
                Debug.LogFormat("PlayerController - Health is empy, freezing player...");
            }
        }
    }

}
