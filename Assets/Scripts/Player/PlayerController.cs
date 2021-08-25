using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Zoca
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameObject localPlayer { get; private set; } 

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
        float ySpeed = 0;


        private void Awake()
        {
            // Get the character controller
            cc = GetComponent<CharacterController>();

            // Set the fireweapon owner
            fireWeapon.SetOwner(this);
            
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            {
                // This is not the local player
                Destroy(playerCamera.gameObject);

                
            }
            else
            {
                // This is the local player
                localPlayer = gameObject;

                // Init player camera
                playerCamera.SetPlayerController(this);

                // Avoid to destroy the player 
                DontDestroyOnLoad(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            
            // Get player team
            Team team = Team.Blue;
            if (PhotonNetwork.IsConnected)
            {
                if(!PlayerCustomPropertyUtility.TryGetPlayerCustomProperty<Team>(PhotonNetwork.LocalPlayer, PlayerCustomProperty.TeamColor, ref team))
                {
                    Debug.LogErrorFormat("PlayerController - property is empty: {0}", PlayerCustomProperty.TeamColor);
                }
                
            }
                

            // Set local player starting position and rotation
            Transform spawnPoint;
            Debug.LogFormat("LocalPlayer has joint the {0} team.", team.ToString());
            if(team == Team.Blue)
            {
                spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
            }
            else
            {
                spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[0];
            }

            Debug.LogFormat("PlayerController - Local player spawn point: {0}", spawnPoint.position);

            cc.Move(spawnPoint.position - cc.transform.position);
            
            //cc.transform.position = spawnPoint.position;
            cc.transform.rotation = spawnPoint.rotation;

        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                Move();

                if (shooting)
                {
                    object[] parameters;
                    if (fireWeapon.TryShoot(out parameters))
                    {
                        Debug.LogFormat("PlayerController - Shoot parameters length: {0}", parameters.Length);
                        for(int i=0; i<parameters.Length; i++)
                            Debug.LogFormat("PlayerController - Shoot parameter[{0}]: {1}", i, parameters[i]);
                        // RPC("Shoot", All, parameters);
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



#region input_system_callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            if (context.performed)
            {
                if (!moving)
                {
                    moving = true;
                    
                    // Start moving
                    //animator.SetBool(walkParam, true);

                    Debug.LogFormat("Player starts moving");
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

                    Debug.LogFormat("Player stops moving");
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
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            // Jump
            if (cc.isGrounded)
            {
                ySpeed = jumpSpeed;
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
        void Move()
        {
            // Look
            Vector2 lookAngles = lookInput * lookSensitivity * Time.deltaTime;
            //Debug.LogFormat("LookAngles: {0}", lookAngles);
            transform.eulerAngles += Vector3.up * lookAngles.x;
            // Camera
            playerCamera.Pitch(-lookAngles.y);


            // Move
            Vector3 dir = transform.forward * input.y + transform.right * input.x;
            targetVelocity = dir.normalized * maxSpeed;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration);

            // Gravity
            if (!cc.isGrounded)
            {
                ySpeed += Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                if (ySpeed < 0)
                    ySpeed = 0;
            }

            velocity.y = ySpeed;

            //transform.position += velocity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);

            //cc.transform.position += velocity * Time.deltaTime;
        }

        [PunRPC]
        void Shoot()
        {

        }
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

    }

}
