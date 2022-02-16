using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Interfaces;

namespace Zoca.UI
{
    public class PowerUpPanel : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Image characterPowerUpImage;

        [SerializeField]
        Image weaponPowerUpImage;

        [SerializeField]
        Transform container;

        PowerUpManager pum;
        List<GameObject> powerUps; // List of templates
        Image characterImageCharge;
        Image weaponImageCharge;
        SkillPowerUp characterPowerUp;
        SkillPowerUp weaponPowerUp;

        #endregion

        #region private methods
        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            // Get the local player power up manager
            pum = PlayerController.Local.GetComponent<PowerUpManager>();

            // Set handles
            pum.OnPowerUpActivated += HandleOnPowerUpActivated;
            pum.OnPowerUpDeactivated += HandleOnPowerUpDeactivated;

            // Set the charge images
            characterImageCharge = characterPowerUpImage.transform.GetChild(0).GetComponent<Image>();
            weaponImageCharge = weaponPowerUpImage.transform.GetChild(0).GetComponent<Image>();
            characterImageCharge.fillAmount = 1;
            weaponImageCharge.fillAmount = 1;

        }

        // Update is called once per frame
        void Update()
        {
            if (!characterPowerUp && !weaponPowerUp)
                return;
            
            if(characterPowerUp != null)
            {
                characterImageCharge.fillAmount = 1 - characterPowerUp.RemainingTime / characterPowerUp.Duration;
            }

            if (weaponPowerUp != null)
            {
                weaponImageCharge.fillAmount = 1 - weaponPowerUp.RemainingTime / weaponPowerUp.Duration;
            }
        }

        void HandleOnPowerUpActivated(IPowerUp powerUp)
        {
            Debug.Log("PowerUpActivated:" + powerUp);

            if (powerUp.GetType() == typeof(SkillPowerUp))
            {
                Debug.Log("PowerUpActivated - is subclass:" + powerUp);
                // We only take into account speed and firePower
                switch ((powerUp as SkillPowerUp).Skill)
                {
                    case Skill.Speed:
                        // Reset character icon timer
                        characterImageCharge.fillAmount = 0;
                        characterPowerUp = powerUp as SkillPowerUp;
                        break;
                    case Skill.FirePower:
                        // Reset character icon timer
                        weaponImageCharge.fillAmount = 0;
                        weaponPowerUp = powerUp as SkillPowerUp;
                        break;
                }
                
            }

        }

        void HandleOnPowerUpDeactivated(IPowerUp powerUp)
        {
            if (!powerUp.GetType().IsSubclassOf(typeof(SkillPowerUp)))
                return;

            if (powerUp as SkillPowerUp == characterPowerUp)
                characterPowerUp = null;

            if (powerUp as SkillPowerUp == weaponPowerUp)
                weaponPowerUp = null;
        }

        
        #endregion
    }

}
