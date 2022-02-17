using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Collections;
using Zoca.Interfaces;

namespace Zoca.UI
{
    public class SpecialPowerUpUI : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Image powerUpImage;

        [SerializeField]
        Image rechargeImage;

        [SerializeField]
        Image emptyImage;

        SpecialSkillPowerUp powerUp;
        List<PowerUpInfo> infoList;
        #endregion

        #region private methods
        private void Awake()
        {
            // Set images 
            // Show the empty image
            Color c = emptyImage.color;
            c.a = 1;
            emptyImage.color = c;
            // Hide the recharge image
            rechargeImage.fillAmount = 0;

            // Load powerup info resources
            infoList = new List<PowerUpInfo>(Resources.LoadAll<PowerUpInfo>(PowerUpInfo.CollectionFolder));
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set local power up manager handles
            PlayerController.Local.GetComponent<PowerUpManager>().OnPowerUpActivated += HandleOnPowerUpActivated;
            PlayerController.Local.GetComponent<PowerUpManager>().OnPowerUpDeactivated += HandleOnPowerUpDeactivated;
        
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!powerUp)
                return;

            rechargeImage.fillAmount = powerUp.CooldownLeft / powerUp.Cooldown;
        }

        IEnumerator PlayPowerInEffect()
        {
            yield return new WaitForEndOfFrame();

            if (!powerUp)
                yield break;

            // Get the info file
            PowerUpInfo info = infoList.Find(i => i.GetPowerUpType() == powerUp.GetType());

            // Set the image
            powerUpImage.sprite = info.Icon;

            // If the empty image is visible set it invisible
            if(emptyImage.color.a != 0)
            {
                emptyImage.DOColor(new Color(1, 1, 1, 0), 1f);
            }

            // Shake the base image
            powerUpImage.transform.DOShakeScale(1);
        }

        IEnumerator PlayPowerOutEffect()
        {
            yield return new WaitForEndOfFrame();

            if (powerUp)
                yield break;

            // Reset empty image
            emptyImage.DOColor(new Color(1, 1, 1, 1), 1f);

            // Shake the base image
            yield return powerUpImage.transform.DOShakeScale(1);

            // Still empty?
            if(!powerUp)
            {
                // If so reset charge image
                rechargeImage.fillAmount = 0;
            }

        }
        #endregion

        #region powerup manager handles
        void HandleOnPowerUpActivated(IPowerUp powerUp)
        {
            if(powerUp.GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)))
            {
                Debug.Log("Special skill has been activated:" + powerUp.GetType());

                this.powerUp = powerUp as SpecialSkillPowerUp;

                StartCoroutine(PlayPowerInEffect());
            }
            
            
        }

        void HandleOnPowerUpDeactivated(IPowerUp powerUp)
        {
            if (powerUp.GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)) && this.powerUp == powerUp as SpecialSkillPowerUp)
            {
                Debug.Log("Special skill has been deactivated:" + powerUp.GetType());
                this.powerUp = null;
                StartCoroutine(PlayPowerOutEffect());
            }
        }
        #endregion

    }

}
