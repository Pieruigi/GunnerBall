using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Interfaces;

namespace Zoca.UI
{
    public class Crosshair : MonoBehaviour
    {
        public static Crosshair Instance { get; private set; }

        [SerializeField]
        Image notAimImage;

        [SerializeField]
        Image aimImage;

        [SerializeField]
        Image notAimLoaderImage;

        [SerializeField]
        Image aimLoaderImage;

        [SerializeField]
        Image superShotImage;

        [SerializeField]
        Transform root;


        PlayerController localPlayerController;
        Collider localPlayerCollider;

        float dotElapsed = 0;
        float dotTime = 0.1f; // Check for dot color every 0.1 seconds
        bool playingSuperShot = false;
        float superShotPlayTime = 0.1f;
        float superShotPlayStrength = 50f;
        SpriteAnimator superShotAnimator;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                superShotAnimator = superShotImage.GetComponent<SpriteAnimator>();
                superShotImage.enabled = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            localPlayerController = PlayerController.Local;
            localPlayerCollider = localPlayerController.GetComponent<CharacterController>();

           
            //notAimImage.transform.DOShakeRotation(1, 50 * Vector3.forward).SetLoops(-1);
            //aimImage.transform.DOShakeRotation(1, 50*Vector3.forward).SetLoops(-1);
        }

        // Update is called once per frame
        void Update()
        {
            CheckDot();

            CheckLoader();

            CheckSuperShot();
        }

        void CheckSuperShot()
        {
            if (PlayerController.Local == null)
                return;

            if (PlayerController.Local.FireWeapon.CheckSuperShot())
            {
                if (!playingSuperShot)
                {
                    playingSuperShot = true;
                    //notAimImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward).OnComplete(HandleOnSuperShootPlayCompleted);
                    //aimImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward);
                    superShotImage.enabled = true;
                    superShotAnimator.Play();
                    superShotImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward).OnComplete(HandleOnSuperShootPlayCompleted);
                }
                
            }
            else
            {
                
                playingSuperShot = false;
                if (superShotAnimator.IsPlaying())
                    superShotAnimator.Stop();

                superShotImage.enabled = false;
            }
        }

        void HandleOnSuperShootPlayCompleted()
        {
            if (!playingSuperShot)
                return;
            //notAimImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward);
            //aimImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward);
            superShotImage.transform.DOShakeRotation(superShotPlayTime, superShotPlayStrength * Vector3.forward).OnComplete(HandleOnSuperShootPlayCompleted);
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

            /***************** Using ray *******************
            localPlayerCollider.enabled = false;
            bool hit = Physics.Raycast(ray, out info, localPlayerController.FireWeapon.FireRange + PlayerController.Local.PlayerCamera.DistanceAdjustment);
            localPlayerCollider.enabled = true;
            ***************************************************/

            /****************** Using sphere ******************************/
            float radius = localPlayerController.FireWeapon.FireRadius;
            int layer = LayerMask.GetMask(new string[] { Layer.Ground, Layer.Wall });
            float maxDistance = localPlayerController.FireWeapon.FireRange + PlayerController.Local.PlayerCamera.DistanceAdjustment - radius;
            bool hit = Physics.SphereCast(ray, radius, out info, maxDistance, ~layer);

            /**************************************************/
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

        public void Show()
        {
            root.gameObject.SetActive(true);
        }
        public void Hide()
        {
            root.gameObject.SetActive(false);
        }
    }

}
