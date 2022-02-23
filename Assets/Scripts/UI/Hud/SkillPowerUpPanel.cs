using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Interfaces;

namespace Zoca.UI
{
    public class SkillPowerUpPanel : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Image characterPowerUpImage;

        [SerializeField]
        Image weaponPowerUpImage;

        PowerUpManager pum;
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
            //Debug.Log("PowerUpActivated:" + powerUp);

            if (powerUp.GetType() == typeof(SkillPowerUp))
            {
                //Debug.Log("PowerUpActivated - is subclass:" + powerUp);
                // We only take into account speed and firePower
                switch ((powerUp as SkillPowerUp).Skill)
                {
                    case Skill.Speed:
                        // Reset character icon timer
                        characterImageCharge.fillAmount = 0;
                        characterPowerUp = powerUp as SkillPowerUp;
                        StartCoroutine(PlayCharacterPowerUpInOut());
                        break;
                    case Skill.FirePower:
                        // Reset character icon timer
                        weaponImageCharge.fillAmount = 0;
                        weaponPowerUp = powerUp as SkillPowerUp;
                        StartCoroutine(PlayWeaponPowerUpInOut());
                        break;
                }
                
            }

        }

        void HandleOnPowerUpDeactivated(IPowerUp powerUp)
        {
            //Debug.Log("Deactivated powerup:" + powerUp);

            if (powerUp.GetType() != typeof(SkillPowerUp))
                return;

            //Debug.Log("powerup:" + powerUp + " is skillPowerUp");

            switch ((powerUp as SkillPowerUp).Skill)
            {
                case Skill.Speed:
                    characterPowerUp = null;
                    characterImageCharge.fillAmount = 1;
                    Debug.Log("powerup:" + powerUp + " is characterpowerup");
                    
                    StartCoroutine(PlayCharacterPowerUpInOut());
                    
                    break;
                case Skill.FirePower:
                    weaponPowerUp = null;
                    weaponImageCharge.fillAmount = 1;
                    Debug.Log("powerup:" + powerUp + " is weaponpowerup");
                    StartCoroutine(PlayWeaponPowerUpInOut());
                    
                    break;

            }
           
                
        }

       
       
        IEnumerator PlayCharacterPowerUpInOut()
        {
            
            yield return new WaitForEndOfFrame();

            characterPowerUpImage.transform.DOShakeScale(1);
            
        }

        IEnumerator PlayWeaponPowerUpInOut()
        {
           
            yield return new WaitForEndOfFrame();

            
            weaponPowerUpImage.transform.DOShakeScale(1);
            
        }

        #endregion
    }

}
