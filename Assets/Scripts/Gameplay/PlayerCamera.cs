using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Zoca
{
    public class PlayerCamera : MonoBehaviour
    {
       
        [SerializeField]
        Transform firstPersonTarget;

        [SerializeField]
        Transform thirdPersonTarget;

        float maxPitch = 30;
        float minPitch = -60;

        float currentPitch;

        PlayerController playerController;
        Material playerMaterial;

        bool thirdPerson = false;
        bool switching = false;
        float switchTime = 1f;
        float switchElapsed = 0;
        float switchSpeed;

        float distanceAdjustment;
        public float DistanceAdjustment
        {
            get { return distanceAdjustment; }
        }

        private void Awake()
        {
            currentPitch = transform.localEulerAngles.x;

            // Move the camera outside
            transform.parent = null;

            // Calculate the switch speed
            switchSpeed = Vector3.Distance(firstPersonTarget.position, thirdPersonTarget.position) / switchTime;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateDistanceAdjustment();
        }

        // Update is called once per frame
        void LateUpdate()
        {

            // If camera is not switching between first and third person modes we
            // can update position and rotation...
            if (!switching)
            {
                if (thirdPerson)
                {
                    UpdateThirdPerson();
                }
                else
                {
                    UpdateFirstPerson();
                }
            }
            else //... otherwise we must lerp between the two modes
            {
                switchElapsed += switchSpeed * Time.deltaTime;

                Vector3 oldTargetPos, newTargetPos;
                Quaternion targetRot;
                if (thirdPerson)
                {
                    oldTargetPos = firstPersonTarget.position;
                    newTargetPos = thirdPersonTarget.position;
                    targetRot = GetThirdPersonTargetRotation();
                }
                else
                {
                    oldTargetPos = thirdPersonTarget.position;
                    newTargetPos = firstPersonTarget.position;
                    targetRot = GetFirstPersonTargetRotation();
                }

                Vector3 targetPos = Vector3.Slerp(oldTargetPos, newTargetPos, switchElapsed / switchTime);
                transform.position = targetPos;
                transform.rotation = targetRot;

                if (switchElapsed > switchTime)
                {
                    switching = false;
                    UpdateDistanceAdjustment();
                }
                    
            }
            
        }

        public void SetPlayerController(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void Pitch(float deltaAngle)
        {
            currentPitch += deltaAngle;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            Vector3 angles = transform.localEulerAngles;
            angles.x = currentPitch;
            transform.localEulerAngles = angles;
        }

        public void Switch()
        {
            switching = true;
            switchElapsed = 0;
            thirdPerson = !thirdPerson;
        }

        public bool IsSwitching()
        {
            return switching;
        }

        public bool IsFirstPerson()
        {
            return !thirdPerson;
        }


        #region private


        /// <summary>
        /// Fire distance is calculated by the player and not by the camera, so
        /// we need to adjust this value depending whether the camera is in
        /// first person mode or not.
        /// </summary>
        void UpdateDistanceAdjustment()
        {
            
            if (!thirdPerson)
            {
                distanceAdjustment = 0;
            }
            else
            {
                Vector3 camToPlayerVector = playerController.transform.position - transform.position;
                distanceAdjustment = Vector3.Dot(camToPlayerVector, transform.forward);
            }
        }

        void UpdateFirstPerson()
        {
            // Adjust position
            transform.position = firstPersonTarget.position;
            
            // Adjust rotation taking into account the current pitch
            transform.rotation = GetFirstPersonTargetRotation();
        }

        void UpdateThirdPerson()
        {
            // Adjust position
            transform.position = thirdPersonTarget.position;

            // Adjust rotation taking into account the current pitch
            transform.rotation = GetThirdPersonTargetRotation();
        }

      

        Quaternion GetFirstPersonTargetRotation()
        {
            Vector3 eulers = transform.localEulerAngles;
            eulers.y = playerController.transform.eulerAngles.y;
            eulers.z = playerController.transform.eulerAngles.z;
            return Quaternion.Euler(eulers);
        }


        Quaternion GetThirdPersonTargetRotation()
        {
            Vector3 eulers = transform.localEulerAngles;
            eulers.y = playerController.transform.eulerAngles.y;
            eulers.z = playerController.transform.eulerAngles.z;
            return Quaternion.Euler(eulers);
        }

        #endregion
    }

}
