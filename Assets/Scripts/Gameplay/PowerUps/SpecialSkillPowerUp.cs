using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zoca.Interfaces;

namespace Zoca
{
    public abstract class SpecialSkillPowerUp : PowerUp, IPickable
    {
        public event UnityAction<IPickable, GameObject> OnPicked;

        #region properties
        public int LeftCharges
        {
            get { return chargeCount; }
        }

        public float CooldownLeft
        {
            get { return Mathf.Max(0, currentCooldown); }
        }

        public float Cooldown
        {
            get { return cooldown; }
        }
        #endregion

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
        protected abstract bool DoTryShoot();
        
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
        public bool TryShoot()
        {
           
            // Can't shoot, return
            if (!CanShoot())
                return false;


            // Must be implemented
            bool shot = DoTryShoot();

            if (shot)
            {
                // Ok, set the cooldown
                currentCooldown = cooldown;

                // Decrease charges
                chargeCount--;
            }

            if (chargeCount == 0)
                Deactivate(Target);

            return shot;
        }

        public bool CanShoot()
        {
            return !(currentCooldown > 0) && chargeCount > 0;
        }
        #endregion

        #region IPickable interface


        public void PickUp(GameObject picker)
        {
            OnPicked?.Invoke(this, picker);

            if (PlayerController.LocalPlayer == picker || PhotonNetwork.OfflineMode)
            {
                // Remove all the children
                int count = transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }

                // Activate the power up
                Activate(picker);
            }

            
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
