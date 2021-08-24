using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Zoca
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        float moveSpeed = 5f;

        [SerializeField]
        GameObject playerCamera;

        Rigidbody rb;

        bool moving = false;
        Vector2 moveDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // Remove camera on others
            if(!GetComponent<PhotonView>().IsMine)
            {
                Destroy(playerCamera);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set player starting position
            Transform spawnPoint;
            Team team = (Team)PhotonNetwork.LocalPlayer.CustomProperties[PlayerCustomProperties.TeamColor];
            Debug.LogFormat("LocalPlayer has joint the {0} team.", team.ToString());
            if(team == Team.Blue)
            {
                spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
            }
            else
            {
                spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[0];
            }
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            //if(moving)
        }


        #region input_system_callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!moving)
                {
                    moving = true;
                    
                    // Start moving
                    //animator.SetBool(walkParam, true);

                    Debug.LogFormat("Player starts moving");
                }

                moveDirection = context.ReadValue<Vector2>().normalized;

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

                

            }

        }

        public void OnPause(InputAction.CallbackContext context)
        {
            GameManager.Instance.Pause();
        }
        #endregion

    }

}
