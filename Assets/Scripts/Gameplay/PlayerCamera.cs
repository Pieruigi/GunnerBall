using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class PlayerCamera : MonoBehaviour
    {
        float maxPitch = 30;
        float minPitch = -60;

        float currentPitch;

        PlayerController playerController;

        private void Awake()
        {
            currentPitch = transform.localEulerAngles.x;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

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
    }

}
