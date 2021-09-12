using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        #region animation_fields
        float animSpeed;
        float animSpeedTarget;
        float animSpeedMax;
        string animSpeedParam = "Speed";

        
        #endregion

        private void Awake()
        {
            if(PlayerController.Local.photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                animSpeedMax = PlayerController.Local.MaxSpeed * PlayerController.Local.SprintMultiplier;
            }
                
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
            if (!PlayerController.Local.photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            // Set animation
            animSpeedTarget = PlayerController.Local.Velocity.magnitude / animSpeedMax;
            //float animSign = Vector3.Dot(velocity.normalized, transform.forward);
            float animSign = PlayerController.Local.MovementInput.y >= 0 ? 1 : -1;
            animSpeedTarget *= animSign;
            animSpeed = Mathf.MoveTowards(animSpeed, animSpeedTarget, 5 * Time.deltaTime);

            animator.SetFloat(animSpeedParam, animSpeed);
        }
    }

}
