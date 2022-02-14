using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    

    public class SkillPowerUp : PowerUp, IPickable
    {
        #region properties
        public float Buff
        {
            get { return buff; }
        }

        //public float Time
        //{
        //    get { return timer; }
        //}

        public Skill Skill
        {
            get { return skill; }
        }
        #endregion

        #region private fields
        [SerializeField]
        float buff = 1.2f;

        [SerializeField]
        float duration = 30;

        [SerializeField]
        Skill skill = Skill.Speed;

        //GameObject target;
        bool loop = false;
        float elapsed = 0;
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // Only update powerups timer while playing
            if (Match.Instance.State != (int)MatchState.Started)
                return;

            if (!loop)
                return;

            

            elapsed += Time.deltaTime;
            if (elapsed > duration)
            {
                // Remove player buff

                // Add to list
                Deactivate(Target);
            }


        }

        void SetSkillValue(Skill skill, float value)
        {
            PlayerController playerController = Target.GetComponent<PlayerController>();
            switch (skill)
            {
                case Skill.FirePower:
                    playerController.FireWeapon.Power = value;
                    break;
                case Skill.FireRange:
                    playerController.FireWeapon.FireRange = value;
                    break;
                case Skill.FireRate:
                    playerController.FireWeapon.FireRate = value;
                    break;
                case Skill.Speed:
                    playerController.MaxSpeed = value;
                    break;
                case Skill.Stamina:
                    playerController.StaminaMax = value;
                    break;
                case Skill.Resistance:
                    playerController.FreezingCooldown = value;
                    break;
            }
        }

        float GetSkillValue(Skill skill)
        {
            PlayerController playerController = Target.GetComponent<PlayerController>();
            float ret = 0;
            switch (skill)
            {
                case Skill.FirePower:
                    ret = playerController.FireWeapon.Power;
                    break;
                case Skill.FireRange:
                    ret = playerController.FireWeapon.FireRange;
                    break;
                case Skill.FireRate:
                    ret = playerController.FireWeapon.FireRate;
                    break;
                case Skill.Speed:
                    ret = playerController.MaxSpeed;
                    break;
                case Skill.Stamina:
                    ret = playerController.StaminaMax;
                    break;
                case Skill.Resistance:
                    ret = playerController.FreezingCooldown;
                    break;
            }

            return ret;
        }

        float GetDefaultSkillValue(Skill skill)
        {
            PlayerController playerController = Target.GetComponent<PlayerController>();
            float ret = 0;
            switch (skill)
            {
                case Skill.FirePower:
                    ret = playerController.FireWeapon.PowerDefault;
                    break;
                case Skill.FireRange:
                    ret = playerController.FireWeapon.FireRangeDefault;
                    break;
                case Skill.FireRate:
                    ret = playerController.FireWeapon.FireRateDefault;
                    break;
                case Skill.Speed:
                    ret = playerController.MaxSpeedDefault;
                    break;
                case Skill.Stamina:
                    ret = playerController.StaminaDefault;
                    break;
                case Skill.Resistance:
                    ret = playerController.FreezingCooldownDefault;
                    break;
            }

            return ret;
        }


        #endregion

        #region public methods
        public void ResetTimer()
        {
            elapsed = 0;
        }
        #endregion

        #region IPickable implementation

        public void PickUp(GameObject picker)
        {
            Activate(picker);
        }

        public bool CanBePicked(GameObject picker)
        {
            return true;
        }


        #endregion

        #region IPowerUp implementation
        public override void Activate(GameObject target)
        {
            // If this power up already exists don't activate it but update timer to the existing one
            List<IPowerUp> list = new List<IPowerUp>(target.GetComponent<PowerUpManager>().PowerUpList);
            // Look for a similar powerup
            SkillPowerUp powerUp = (SkillPowerUp)list.Find(p => p.GetType() == typeof(SkillPowerUp) && (p as SkillPowerUp).skill == this.skill);
            if (!powerUp) // Not found
            {

                base.Activate(target);

                PlayerController playerController = target.GetComponent<PlayerController>();

                SetSkillValue(skill, GetSkillValue(skill) + (GetDefaultSkillValue(skill) * buff));

                loop = true;
            }
            else // Already exists
            {
                // Simply reset the timer
                powerUp.ResetTimer();
                // Destroy this power up by calling the base method
                base.Deactivate(target);
            }
            
        }

        public override void Deactivate(GameObject target)
        {
            
            SetSkillValue(skill, GetSkillValue(skill) - (GetDefaultSkillValue(skill) * buff));

            base.Deactivate(target);
        }
        #endregion
    }

}
