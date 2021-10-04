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
        Image dotImage;

        [SerializeField]
        Image dotImageSmall;

        [SerializeField]
        Image loaderImage;

        PlayerController localPlayerController;
        Collider localPlayerCollider;

        float dotElapsed = 0;
        float dotTime = 0.1f; // Check for dot color every 0.1 seconds

        // Start is called before the first frame update
        void Start()
        {
            localPlayerController = PlayerController.Local;
            localPlayerCollider = localPlayerController.GetComponent<Collider>();
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
            localPlayerCollider.enabled = false;
            bool hit = Physics.Raycast(ray, out info, localPlayerController.FireWeapon.FireRange + PlayerController.Local.PlayerCamera.DistanceAdjustment);
            localPlayerCollider.enabled = true;

            Color c = Color.red;
            dotImage.enabled = true;
            dotImageSmall.enabled = false;

            if (hit && info.collider.GetComponent<IHittable>() != null)
            {
                c = Color.green;
                dotImageSmall.enabled = true;
                dotImage.enabled = false;
            }

            // Set color
            //dotImage.color = c;
        }

        void CheckLoader()
        {
            float elapsed = Mathf.Max(localPlayerController.FireWeapon.CooldownElapsed, 0f);
            float r = elapsed / localPlayerController.FireWeapon.Cooldown;

            loaderImage.fillAmount = r;

            if(r > 0)
            {
                loaderImage.color = Color.red;
            }
            else
            {
                loaderImage.color = Color.green;
            }
        }
    }

}
