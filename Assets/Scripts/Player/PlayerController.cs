using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Zoca
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float moveSpeed = 5f;

        Rigidbody rb;

        bool moving = false;
        Vector2 moveDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {

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

        public void OnExit(InputAction.CallbackContext context)
        {
            GameManager.Instance.Exit();
        }
        #endregion

    }

}
