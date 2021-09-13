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

        // Player controller should pass this array
        [SerializeField]
        Renderer[] renderers;

        float maxPitch = 30;
        float minPitch = -60;

        float currentPitch;
        public float CurrentPitch
        {
            get { return currentPitch; }
        }

        PlayerController playerController;
        Material playerMaterial;

        bool thirdPerson = true;
        bool switching = false;
        float switchTime = 1f;
        float switchElapsed = 0;
        float switchSpeed;

        float distanceAdjustment;
        public float DistanceAdjustment
        {
            get { return distanceAdjustment; }
        }

        List<Material> originalMaterials;
        List<Material> transparentMaterials;

        private void Awake()
        {
            currentPitch = transform.localEulerAngles.x;

            // Move the camera outside
            transform.parent = null;

            // Calculate the switch speed
            switchSpeed = Vector3.Distance(firstPersonTarget.position, thirdPersonTarget.position) / switchTime;

            // Store original and create transparent materials
            originalMaterials = new List<Material>();
            transparentMaterials = new List<Material>();
            foreach(Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    originalMaterials.Add(mat);
                    Material tMat = new Material(mat);
                    tMat.SetFloat("_Surface", 1);
                    transparentMaterials.Add(tMat);

                    
                }
                
            }


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
                    //SetMaterials(originalMaterials);
                }
                    
            }
            
        }

        public void SetPlayerController(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void SetPitch(float pitch)
        {
            currentPitch = pitch;
            Vector3 angles = transform.localEulerAngles;
            angles.x = currentPitch;
            transform.localEulerAngles = angles;
        }

        public void Switch()
        {
            switching = true;
            switchElapsed = 0;
            thirdPerson = !thirdPerson;

            // Change material to transparent
            StartCoroutine(DoTransparentEffect());
            //SetMaterials(transparentMaterials);

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
            float time = switchTime / 2;
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

        #endregion
    }

}
