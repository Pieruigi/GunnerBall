//#define SYNC_MOVE_INPUT
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class AnimatorController : MonoBehaviour, IPunObservable
    {
        [SerializeField]
        Animator animator;

        PlayerController playerController;

        #region animation_fields
        float animSpeed;
        float animSpeedTarget;
        float animSpeedMax;
        string animSpeedParam = "Speed";
        int animSpeedSign;
        
        #endregion

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            animSpeedMax = playerController.MaxSpeed * playerController.SprintMultiplier;

           
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            
            // Set animation
            animSpeedTarget = playerController.Velocity.magnitude / animSpeedMax;
            //float animSign = Vector3.Dot(velocity.normalized, transform.forward);

#if !SYNC_MOVE_INPUT
            if (playerController.photonView.IsMine || PhotonNetwork.OfflineMode)
#endif
            {
                animSpeedSign = playerController.MovementInput.y >= 0 ? 1 : -1;
            }
            
            
            animSpeedTarget *= animSpeedSign;
            animSpeed = Mathf.MoveTowards(animSpeed, animSpeedTarget, 5 * Time.deltaTime);

            if (!playerController.photonView.IsMine)
            {
                Debug.LogFormat("AnimatorController - AnimSign: {0}", animSpeedSign);
                Debug.LogFormat("AnimatorController - AnimSpeed: {0}", animSpeed);
            }
                

            animator.SetFloat(animSpeedParam, animSpeed);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
#if !SYNC_MOVE_INPUT
            if (stream.IsWriting)
            {
                stream.SendNext((byte)animSpeedSign);
            }
            else
            {
                animSpeedSign = (byte)stream.ReceiveNext();
                if (animSpeedSign == 255)
                    animSpeedSign = -1;
            }
#endif
        }
    }

}
