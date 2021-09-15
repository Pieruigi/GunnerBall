//#define SYNC_MOVE_INPUT
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class AnimationController : MonoBehaviour
#if !SYNC_MOVE_INPUT
        , IPunObservable
#endif
    {
        [SerializeField]
        Animator animator;

        [Header("Nodes")]
        [SerializeField]
        Transform root;

        [SerializeField]
        Transform[] spines;

        [SerializeField]
        Transform leftShoulder;

        [SerializeField]
        Transform rightShoulder;

        [Header("Strafe")]
        [SerializeField]
        float strafeAngleMax = 60f;

        [SerializeField]
        float spineStrafeAngleMultiplyer = .2f;

        [Header("Aim")]
        [SerializeField]
        float aimAngleMax = 60;

        [SerializeField]
        float aimAngleMin = 60;

        PlayerController playerController;

#region motion_fields
        float animSpeed;
        float animSpeedTarget;
        float animSpeedMax;
        string animSpeedParam = "Speed";
        int animSpeedSign;
        #endregion

        #region strafe_fields
        float targetStrafeAngle;
        float currentStrafeAngle;
        #endregion

        #region aim_fields
        float targetPitch;
        float currentPitch;
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
            AnimateStrafe();
            AnimateMotion();
            AnimateAim();
        }

        #region private_animation_methods
        void AnimateMotion()
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

            //if (!playerController.photonView.IsMine)
            //{
            //    Debug.LogFormat("AnimatorController - AnimSign: {0}", animSpeedSign);
            //    Debug.LogFormat("AnimatorController - AnimSpeed: {0}", animSpeed);
            //}


            animator.SetFloat(animSpeedParam, animSpeed);
        }

        void AnimateStrafe()
        {
            // When moving straight forward Dot(fwd, dir) = 1; instead
            // when strafing Dot(fwd, dir) = 0.
            // Local player only
#if !SYNC_MOVE_INPUT
            if (playerController.photonView.IsMine || PhotonNetwork.OfflineMode)
#endif
            {
                float y = playerController.MovementInput.y;
                float x = playerController.MovementInput.x;
                if ((x != 0 || y != 0) && !playerController.Sprinting)
                {
                    // Set the target angle
                    targetStrafeAngle = Mathf.Lerp(0, strafeAngleMax, 1.0f - Mathf.Abs(y)) * Mathf.Sign(x) * Mathf.Sign(y);
                }
                else
                {
                    // Reset the target angle
                    targetStrafeAngle = 0;
                }
            }

            // For both local and remote player
            // Get the current angle
            currentStrafeAngle = Mathf.MoveTowards(currentStrafeAngle, targetStrafeAngle, 360 * Time.deltaTime);

            Vector3 eulers;

            // Rotate the root node
            eulers = root.localEulerAngles;
            eulers.y = currentStrafeAngle;
            root.localEulerAngles = eulers;

            float delta = -currentStrafeAngle * spineStrafeAngleMultiplyer;

            // Rotate spines
            for (int i = 0; i < spines.Length; i++)
            {
                eulers = spines[i].transform.localEulerAngles;
                eulers.y = delta;
                spines[i].transform.localEulerAngles = eulers;
            }

        }

        void AnimateAim()
        {
            currentPitch = playerController.CurrentPitch;

            leftShoulder.RotateAround(leftShoulder.position, transform.right, currentPitch);
            rightShoulder.RotateAround(rightShoulder.position, transform.right, currentPitch);
        }

        #endregion

#if !SYNC_MOVE_INPUT
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting)
            {
                stream.SendNext((byte)animSpeedSign);
                stream.SendNext(targetStrafeAngle);
                
            }
            else
            {
                animSpeedSign = (byte)stream.ReceiveNext();
                targetStrafeAngle = (float)stream.ReceiveNext();
                
                if (animSpeedSign == 255)
                    animSpeedSign = -1;
            }

        }
#endif
    }

}
