using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Interfaces;

namespace Zoca.UI
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField]
        Image notAimImage;

        [SerializeField]
        Image aimImage;

        [SerializeField]
        Image notAimLoaderImage;

        [SerializeField]
        Image aimLoaderImage;


        PlayerController localPlayerController;
        Collider localPlayerCollider;

        float dotElapsed = 0;
        float dotTime = 0.1f; // Check for dot color every 0.1 seconds

        // Start is called before the first frame update
        void Start()
        {
            localPlayerController = PlayerController.Local;
            localPlayerCollider = localPlayerController.GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckDot();

            CheckLoader();
        }

        void CheckDot()
        {
            if (PlayerController.Local == null)
                return;

            dotElapsed += Time.deltaTime;
            if (dotElapsed < dotTime)
                return;

            dotElapsed = 0;

            // Cast a ray
            // Get origin, direction and speed
            Vector3 origin = localPlayerController.PlayerCamera.transform.position;
            Vector3 direction = localPlayerController.PlayerCamera.transform.forward;

            // Check for collision
            Ray ray = new Ray(origin, direction);
            RaycastHit info;
            //int mask = ~LayerMask.NameToLayer(Layer.Player);
            localPlayerCollider.enabled = false;
            bool hit = Physics.Raycast(ray, out info, localPlayerController.FireWeapon.FireRange + PlayerController.Local.PlayerCamera.DistanceAdjustment);
            localPlayerCollider.enabled = true;

            Color c = Color.red;
            notAimImage.enabled = true;
            notAimLoaderImage.enabled = true;
            aimImage.enabled = false;
            aimLoaderImage.enabled = false;

            if (hit && info.collider.GetComponent<IHittable>() != null)
            {
                c = Color.green;
                aimImage.enabled = true;
                aimLoaderImage.enabled = true;
                notAimImage.enabled = false;
                notAimLoaderImage.enabled = false;
            }

            // Set color
            //dotImage.color = c;
        }

        void CheckLoader()
        {
            float elapsed = Mathf.Max(localPlayerController.FireWeapon.CooldownElapsed, 0f);
            float r = elapsed / localPlayerController.FireWeapon.Cooldown;

            notAimLoaderImage.fillAmount = r;
            aimLoaderImage.fillAmount = r;

            //if (r > 0)
            //{
            //    loaderImage.color = Color.red;
            //}
            //else
            //{
            //    loaderImage.color = Color.green;
            //}
        }
    }

}
