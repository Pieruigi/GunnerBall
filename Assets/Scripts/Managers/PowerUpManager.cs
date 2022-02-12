using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Zoca
{


    public class PowerUpManager : MonoBehaviour
    {
        #region events
        public UnityAction<Skill> OnPowerUpActivated;
        public UnityAction<Skill> OnPowerUpDeactivated;
        #endregion

        #region internal classes
        class Data
        {
            public float buff;
            public float time;
            public Skill skill;
            public float elapsed;
            
            public Data(float buff, float time, Skill skill) 
            { 
                this.buff = buff; 
                this.time = time;
                this.skill = skill;
                elapsed = 0;
            }
        }
        #endregion

        #region properties
        public int PowerUpCount
        {
            get { return datas.Count; }
        }
        #endregion

        #region private
        const int PowerUpMax = 10;

        List<Data> datas = new List<Data>(PowerUpMax);

        PlayerController playerController;

        // All the skills
        float speedDefault;
        float staminaDefault;
        float resistanceDefault;
        float firePowerDefault;
        float fireRateDefault;
        float fireRangeDefault;
        #endregion

        #region private methods
        private void Awake()
        {
            // Clear all
            Reset();

            playerController = GetComponent<PlayerController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set default skills
            InitDefaultSkills();
        }

        // Update is called once per frame
        void Update()
        {
            // Only update powerups timer while playing
            if (Match.Instance.State != (int)MatchState.Started)
                return;

            // Check for each power up active on this player
            List<Data> toRemoveList = new List<Data>(); // We can't remove element while in loop
            foreach(Data data in datas)
            {
                // Update the elapsed time
                data.elapsed += Time.deltaTime;
                if(data.elapsed > data.time)
                {
                    // Remove player buff
                    SetSkillValue(data.skill, GetSkillValue(data.skill) - (GetSkillValueDefault(data.skill) * data.buff));
                    // Add to list
                    toRemoveList.Add(data);
                }
            }

            // Remove expired powerups
            foreach (Data data in toRemoveList)
            {
                datas.Remove(data);

                // Call event for each powerup
                OnPowerUpDeactivated?.Invoke(data.skill);
            }
                
        }

        void InitDefaultSkills()
        {
            speedDefault = playerController.MaxSpeed;
            staminaDefault = playerController.StaminaMax;
            resistanceDefault = playerController.FreezingCooldown;
            firePowerDefault = playerController.FireWeapon.Power;
            fireRateDefault = playerController.FireWeapon.FireRate;
            fireRangeDefault = playerController.FireWeapon.FireRange;
            
        }

        private void Reset()
        {
            datas.Clear();
        }

        void SetSkillValue(Skill skill, float value)
        {
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

        float GetSkillValueDefault(Skill skill)
        {
            float ret = 0;
            switch (skill)
            {
                case Skill.FirePower:
                    ret = firePowerDefault;
                    break;
                case Skill.FireRange:
                    ret = fireRangeDefault;
                    break;
                case Skill.FireRate:
                    ret = fireRateDefault;
                    break;
                case Skill.Speed:
                    ret = speedDefault;
                    break;
                case Skill.Stamina:
                    ret = staminaDefault;
                    break;
                case Skill.Resistance:
                    ret = resistanceDefault;
                    break;
            }

            return ret;
        }
       
        #endregion

        #region public methods
        public void PowerUp(PowerUp powerUp)
        {
            Debug.Log("Activating power up");
            Data data = datas.Find(d => d.skill == powerUp.Skill);
            if (data == null)
            {
                // Power up skill
                SetSkillValue(powerUp.Skill, GetSkillValue(powerUp.Skill) + (GetSkillValueDefault(powerUp.Skill) * powerUp.Buff));
                datas.Add(new Data(powerUp.Buff, powerUp.Time, powerUp.Skill));
            }
            else
            {
                // Skill already powered up, just reset the timer
                data.elapsed = 0;
            }

            OnPowerUpActivated?.Invoke(powerUp.Skill);
        }



        public bool CanBePoweredUp(PowerUp powerUp)
        {
            //// Only one powerup of a given type can be active
            //foreach(Data data in datas)
            //{
            //    if (data.skill == powerUp.Skill)
            //        return false;
            //}

            return true;
            //return datas.Count < PowerUpMax ? true : false;
        }

        public bool IsPowerUpActive(Skill skill)
        {
            return datas.Exists(p => p.skill == skill);
        }

        public float GetPowerUpRemainingTime(Skill skill)
        {
            Data data = datas.Find(p => p.skill == skill);
            if (data == null)
                return 0;

            return Mathf.Max(0, data.time - data.elapsed);
        }

        public float GetPowerUpTime(Skill skill)
        {
            Data data = datas.Find(p => p.skill == skill);
            if (data == null)
                return 0;

            return data.time;
        }
        #endregion
    }

}
