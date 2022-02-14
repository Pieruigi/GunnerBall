using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public abstract class SpecialSkillPowerUp : PowerUp, IPickable
    {
        #region private fields
        [SerializeField]
        int chargeCount; // How many times you can use this powerup before it expires

        [SerializeField]
        float cooldown;
        float currentCooldown = 0;

       
        #endregion

        #region abstract methods
        /// <summary>
        /// Implement this to shoot the powerup
        /// </summary>
        protected abstract void DoShoot();
        
        #endregion

        #region private methods
        

        // Update is called once per frame
        protected override void Update()
        {
            if(!CanShoot())
            {
                currentCooldown -= Time.deltaTime;

            }
        }
        #endregion

        #region public methods
        public void Shoot()
        {
            // Can't shoot, return
            if (!CanShoot())
                return;

            // Ok, set the cooldown
            currentCooldown = cooldown;

            // Must be implemented
            DoShoot();
        }

        public bool CanShoot()
        {
            return !(currentCooldown > 0);
        }
        #endregion

        #region IPickable interface


        public void PickUp(GameObject picker)
        {
            // Remove all the children
            int count = transform.childCount;
            for(int i=0; i<count; i++)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

            // Activate the power up
            Activate(picker);
        }

        public bool CanBePicked(GameObject picker)
        {
            //// Get the powerup manager
            //PowerUpManager pum = picker.GetComponent<PowerUpManager>();

            //// If the the player already has a powerup of the same type active you can't pick this up
            //if (new List<IPowerUp>(pum.PowerUpList).Exists(p => p.GetType().IsSubclassOf(this.GetType())))
            //    return false;

            return true;
        }
        #endregion
        #region IPowerUp interface
        public override void Activate(GameObject target)
        {
            Debug.Log("This-TypeOf:" + GetType());
            Debug.Log("This-TypeOf.IsSubclass:" + GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)));

            PowerUpManager pum = target.GetComponent<PowerUpManager>();

            // If there is another powerup of the same type active then we remove it
            SpecialSkillPowerUp ssp = (SpecialSkillPowerUp)new List<IPowerUp>(pum.PowerUpList).Find(p => p.GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)));

            if (ssp) // Already exists, destroy it
            {
                ssp.Deactivate(target);
            }
            currentCooldown = 0.5f;
            base.Activate(target);
        }

        public override void Deactivate(GameObject target)
        {
            base.Deactivate(target);
        }
        #endregion
    }

}
