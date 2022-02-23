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
                        StartCoroutine(PlaySkillPowerUpIn(characterPowerUp));
                        break;
                    case Skill.FirePower:
                        // Reset character icon timer
                        weaponImageCharge.fillAmount = 0;
                        weaponPowerUp = powerUp as SkillPowerUp;
                        StartCoroutine(PlaySkillPowerUpIn(weaponPowerUp));
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
                    characterImageCharge.fillAmount = 1;
                    Debug.Log("powerup:" + powerUp + " is characterpowerup");
                    
                    StartCoroutine(PlaySkillPowerUpOut(characterPowerUp));
                    characterPowerUp = null;
                    break;
                case Skill.FirePower:
                    
                    weaponImageCharge.fillAmount = 1;
                    Debug.Log("powerup:" + powerUp + " is weaponpowerup");
                    StartCoroutine(PlaySkillPowerUpOut(weaponPowerUp));
                    weaponPowerUp = null;
                    break;

            }
           
                
        }

       
       
        IEnumerator PlaySkillPowerUpIn(SkillPowerUp powerUp)
        {
            Debug.Log("PowerUp In:" + powerUp);

            yield return new WaitForEndOfFrame();
            if (!powerUp)
                yield break;
            if (powerUp == characterPowerUp)
            {
                characterPowerUpImage.transform.DOShakeScale(1);
            }
            else
            {
                weaponPowerUpImage.transform.DOShakeScale(1);
            }
            
        }

        IEnumerator PlaySkillPowerUpOut(SkillPowerUp powerUp)
        {
            Debug.Log("PowerUp Out:" + powerUp);
            yield return new WaitForEndOfFrame();

            if (powerUp)
                yield break;

            if (powerUp == characterPowerUp)
            {
                characterPowerUpImage.transform.DOShakeScale(1);
            }
            else
            {
                weaponPowerUpImage.transform.DOShakeScale(1);
            }

        }

        #endregion
    }

}
