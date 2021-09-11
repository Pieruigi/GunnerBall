using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class StrafeController : MonoBehaviour
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
            Vector3 velocity = Vector3.zero;
            if (playerController.IsMoving())
            {
                velocity = playerController.Velocity;
                velocity.y = 0; // To be sure
                float dot = Vector3.Dot(transform.forward, velocity.normalized);
                float dotRight = Vector3.Dot(transform.right, velocity.normalized);
                Debug.Log("Dot:" + dot);
                // Set the target angle
                targetAngle = Mathf.Lerp(0, maxAngle, 1.0f - Mathf.Abs(dot)) * Mathf.Sign(dotRight);
            }
            else
            {
                // Reset the target angle
                targetAngle = 0;
            }
           
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
    }

}
