using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class StrafeController : MonoBehaviour, IPunObservable
    {
        [SerializeField]
        Transform root;
      
        [SerializeField]
        Transform[] spines;

        [SerializeField]
        float maxAngle = 60f;

        [SerializeField]
        float spineAngleMultiplyer = .2f;

        PlayerController playerController;

        float targetAngle;
        float currentAngle;

        Vector3 oldPosition;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
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
            // When moving straight forward Dot(fwd, dir) = 1; instead
            // when strafing Dot(fwd, dir) = 0.
            // Local player only
            if (playerController.photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                float y = playerController.MovementInput.y;
                float x = playerController.MovementInput.x;
                if (x != 0 || y != 0)
                {
                    // Set the target angle
                    targetAngle = Mathf.Lerp(0, maxAngle, 1.0f - Mathf.Abs(y)) * Mathf.Sign(x) * Mathf.Sign(y);
                }
                else
                {
                    // Reset the target angle
                    targetAngle = 0;
                }
            }
            
            // For both local and remote player
            // Get the current angle
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 360 * Time.deltaTime);
            
            Vector3 eulers;
            
            // Rotate the root node
            eulers = root.localEulerAngles;
            eulers.y = currentAngle;
            root.localEulerAngles = eulers;

            float delta = -currentAngle * spineAngleMultiplyer;

            // Rotate spines
            for (int i=0; i<spines.Length; i++)
            {
                eulers = spines[i].transform.localEulerAngles;
                eulers.y = delta;
                spines[i].transform.localEulerAngles = eulers;
            }

            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(targetAngle);
            }
            else
            {
                targetAngle = (float)stream.ReceiveNext();
            }
        }
    }

}
