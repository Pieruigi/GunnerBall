using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Zoca
{
    public class PlayerCamera : MonoBehaviour
    {
        //public static PlayerCamera Instance { get; private set; }

        [SerializeField]
        Transform target;

        // Player controller should pass this array
        [SerializeField]
        Renderer[] renderers;

        //float maxPitch = 30;
        //float minPitch = -60;

        float currentPitch;
        public float CurrentPitch
        {
            get { return currentPitch; }
        }

        PlayerController playerController;
        Material playerMaterial;

        float distanceAdjustment;
        public float DistanceAdjustment
        {
            get { return distanceAdjustment; }
        }

        List<Material> originalMaterials;
        List<Material> transparentMaterials;

        Collider playerCollider;

        Vector3 targetPositionOnSprint;
        Vector3 targetPositionDefault;
        float onSprintLerpSpeed = 0.8f;

        private void Awake()
        {
            //if (!Instance)
            //{
                //Instance = this;
                currentPitch = transform.localEulerAngles.x;

               
                // Move the camera outside
                transform.parent = null;

                // Set the camera target position on sprint
                targetPositionOnSprint = target.localPosition + Vector3.forward * 0.57f;

                // Store the default position
                targetPositionDefault = target.localPosition;

                // Store original and create transparent materials
                originalMaterials = new List<Material>();
                transparentMaterials = new List<Material>();
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        originalMaterials.Add(mat);
                        Material tMat = new Material(mat);
                        tMat.SetFloat("_Surface", 1);
                        transparentMaterials.Add(tMat);


                    }

                }
            //}
            //else
            //{
            //    Destroy(gameObject);
            //}



        }

        // Start is called before the first frame update
        void Start()
        {
            


            UpdateDistanceAdjustment();
        }

        private void Update()
        {
            
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!playerController)
                return;

            // On sprint we move the camera target closer to the player
            if (playerController.photonView.IsMine)
                CheckPlayerSprint();

            // Update the camera position and rotation
            UpdatePositionAndRotation();
          
            // Clip camera
            if(playerController.photonView.IsMine)
                DoClipping();
        }

      

      
      
        #region private


        /// <summary>
        /// Fire distance is calculated by the player and not by the camera; in this way we can add more
        /// zoom without interfering with gameplay
        /// </summary>
        void UpdateDistanceAdjustment()
        {
            
            Vector3 camToPlayerVector = playerController.transform.position - transform.position;
            distanceAdjustment = Vector3.Dot(camToPlayerVector, transform.forward);
            
        }

        /// <summary>
        /// This method sets the appropriate camera target depending wheter the player is sprinting or not.
        /// </summary>
        void CheckPlayerSprint()
        {
        
            if (playerController.Sprinting)
            {
                target.localPosition = Vector3.MoveTowards(target.localPosition, targetPositionOnSprint, onSprintLerpSpeed * Time.deltaTime);
            }
            else
            {
                target.localPosition = Vector3.MoveTowards(target.localPosition, targetPositionDefault, onSprintLerpSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Update position and rotation
        /// </summary>
        void UpdatePositionAndRotation()
        {
            // Adjust position
            transform.position = target.position;

            // Adjust rotation taking into account the current pitch
            //transform.rotation = GetThirdPersonTargetRotation();
            // Reset transform
            transform.forward = playerController.transform.forward;

            // Add pitch
            Vector3 eulers = transform.localEulerAngles;
            eulers.x = currentPitch;
            transform.localEulerAngles = eulers;
        }

      

   


        Quaternion GetThirdPersonTargetRotation()
        {
            Vector3 eulers = transform.localEulerAngles;
            eulers.y = playerController.transform.eulerAngles.y;
            eulers.z = playerController.transform.eulerAngles.z;
            return Quaternion.Euler(eulers);
        }

        void SetMaterials(List<Material> newMaterials)
        {
            
            int startIdx = 0;
            foreach (Renderer rend in renderers)
            {
                // The number of materials of the current renderer
                int count = rend.materials.Length;

                // Create a new material array
                Material[] mats = new Material[count];

                // Fill the array
                for(int i=0; i<count; i++)
                {
                    mats[i] = newMaterials[i + startIdx];
                }

                // Set materials
                rend.materials = mats;
                rend.UpdateGIMaterials();

                startIdx += count;
            }
        }

        IEnumerator DoTransparentEffect()
        {
            SetMaterials(transparentMaterials);
            float time = 0.5f;
            float elapsed = 0;

            
            while(elapsed < time)
            {
                elapsed += Time.deltaTime;

                foreach(Material mat in transparentMaterials)
                {
                    Color c = mat.color;
                    c.a = Mathf.Lerp(1, 0, elapsed / time);
                    mat.color = c;
                }

                yield return null;
            }
            
        }

        void DoClipping()
        {
            // Cast a ray from the player to the camera and set the camera to the 
            // collision point if any
            // Calculate the origin
            Vector3 origin = playerController.transform.position;
            origin.y = target.position.y;
            // The direction
            Vector3 fromPlayerToCamera = transform.position - origin; 
            // The ray
            Ray ray = new Ray(origin, fromPlayerToCamera.normalized);

            // Disable player collision
            playerCollider.enabled = false;
            // Cast a ray
            RaycastHit info;
            if(Physics.Raycast(ray, out info, fromPlayerToCamera.magnitude))
            {
                // Clipping, replace camera
                Vector3 newPos = info.point - fromPlayerToCamera.normalized * .2f;
                newPos.y = target.position.y;
                transform.position = newPos;
                
            }

            // Enable player collider 
            playerCollider.enabled = true;
        }
        #endregion

        #region public
        public void SetPlayerController(PlayerController playerController)
        {
            this.playerController = playerController;
            playerCollider = playerController.GetComponent<Collider>();

            Debug.Log("PlayerCamera.ViewId:" + playerController.photonView.ViewID);
            Debug.Log("PlayerCamera.Owner:" + playerController.photonView.Owner);
            Debug.Log("PlayerCamera.OwnerActorNr:" + playerController.photonView.OwnerActorNr);
            Debug.Log("PlayerCamera.AmController:" + playerController.photonView.AmController);
            Debug.Log("PlayerCamera.AmOwner:" + playerController.photonView.AmOwner);
            Debug.Log("PlayerCamera.ControllerActornNr:" + playerController.photonView.ControllerActorNr);

            if (!playerController.photonView.IsMine || playerController.photonView.IsRoomView)
            {
                Destroy(GetComponent<AudioListener>());
                Destroy(GetComponent<UniversalAdditionalCameraData>());
                Destroy(GetComponent<Camera>());

                // Move the camera pivot behind
                target.localPosition = new Vector3(0, target.localPosition.y, target.localPosition.z);
                targetPositionOnSprint = target.localPosition;

                //// Store the default position
                targetPositionDefault = target.localPosition;
            }
        }

        public void SetPitch(float pitch)
        {
            currentPitch = pitch;
            //Vector3 angles = transform.localEulerAngles;
            //angles.x = currentPitch;
            //transform.localEulerAngles = angles;
        }
        #endregion
    }

}
