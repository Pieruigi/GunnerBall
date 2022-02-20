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

        [Header("Strafe")]
        [SerializeField]
        float strafeAngleMax = 60f;

        [SerializeField]
        float spineStrafeAngleMultiplyer = .2f;

        
        PlayerController playerController;
        CharacterController cc;

        #region motion_fields
        float animSpeed;
        float animSpeedTarget;
        float animSpeedMax;
        string animSpeedParam = "Speed";
        int animSpeedSign;
        #endregion

        #region shoot
        string shootSpeedParam = "ShootSpeed";
        string shootParam = "Shoot";
        #endregion

        #region strafe_fields
        float targetStrafeAngle;
        float currentStrafeAngle;
        #endregion

        #region aim_fields
        float targetPitch;
        float currentPitch;
        float pitchMultiplier = 0.7f;
        #endregion

        #region turn_around
        int animTurnDirection = 0;
        string animTurnDirectionParam = "TurnDirection";
        float oldEulerY = 0;
        string animTurnSpeedParam = "TurnSpeed";

        #endregion

        #region freeze
        string freezeParam = "Freeze";
        #endregion

        

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            animSpeedMax = playerController.MaxSpeed * playerController.SprintMultiplier;
            cc = GetComponent<CharacterController>();
            
        }

        // Start is called before the first frame update
        void Start()
        {
            oldEulerY = transform.eulerAngles.y;

          
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {

            AnimateTurnAround();
            
            AnimateStrafe();
            AnimateMotion();
            AnimateAim();
        }

        public void AnimateShoot(float speed)
        {
            animator.SetFloat(shootSpeedParam, speed);
            animator.SetTrigger(shootParam);
        }

        public void AnimateFreeze(bool value)
        {
            if (animator.GetBool(freezeParam) == value)
                return;

            animator.SetBool(freezeParam, value);
            
        }

        #region private_animation_methods
        void AnimateTurnAround()
        {
            
            if(playerController.Velocity.magnitude == 0)
            {
                
                // Start rotating around
                float turnAngle = transform.eulerAngles.y - oldEulerY;
                float turnSpeed = turnAngle / Time.deltaTime;
                turnSpeed /= 90f; // The anim rotates 90 degrees per second
                //Debug.LogFormat("AnimationController - TurnSpeed: {0}", turnSpeed);
                // Set the animation speed
                animator.SetFloat(animTurnSpeedParam, turnSpeed);
                //Debug.LogFormat("AnimateTurnAround - turn angle: {0}", turnAngle);
                int animValue = animator.GetInteger(animTurnDirectionParam);
                if (turnAngle > 0 && animValue != 1)
                {
                    animator.SetInteger(animTurnDirectionParam, 1);
                }
                else
                {
                    if(turnAngle < 0 && animValue != -1)
                    {
                        animator.SetInteger(animTurnDirectionParam, -1);
                    }
                    else
                    {
                        if(turnAngle == 0 && animValue != 0)
                        {
                            animator.SetInteger(animTurnDirectionParam, 0);
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("Player is moving - vel:" + playerController.Velocity);
                animator.SetInteger(animTurnDirectionParam, 0);
            }
            oldEulerY = transform.eulerAngles.y;
        }

        void AnimateMotion()
        {
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

            // Set animation
            animSpeedTarget = playerController.Velocity.magnitude / animSpeedMax;
            //float animSign = Vector3.Dot(velocity.normalized, transform.forward);

            if (playerController.Jumping)
            {
                //Debug.Log("JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ");
                animSpeedTarget *= 0.2f;
            }

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
            eulers.y += currentStrafeAngle;
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
            Debug.Log("CurrentPitch:" + currentPitch);
            float animPitch = currentPitch * pitchMultiplier;
          
            spines[2].RotateAround(spines[2].position, transform.right, animPitch);
            
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
