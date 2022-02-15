using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zoca.Interfaces;

namespace Zoca
{


    public class PowerUpManager : MonoBehaviour
    {
        #region events
        public UnityAction<Skill> OnPowerUpActivated;
        public UnityAction<Skill> OnPowerUpDeactivated;
        #endregion

        #region internal classes
    
            
      
        #endregion

        #region properties
       
        #endregion

        #region private
      

        PlayerController playerController;

        List<IPowerUp> powerUpList = new List<IPowerUp>();

        public IList<IPowerUp> PowerUpList
        {
            get { return powerUpList.AsReadOnly(); }
        }

     
        #endregion

        #region private methods
        private void Awake()
        {
            // Clear all
            //Reset();

            playerController = GetComponent<PlayerController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
           
          
                
        }

       
       
        #endregion

        #region public methods
        public void Add(IPowerUp powerUp)
        {
            powerUpList.Add(powerUp);
        }

        public void Remove(IPowerUp powerUp)
        {
            powerUpList.Remove(powerUp);
        }

        public bool HasSpecialSkillPowerUp()
        {
            return powerUpList.Exists(p => p.GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)));
        }
 
        public SpecialSkillPowerUp GetSpecialSkillPowerUp()
        {
            return (SpecialSkillPowerUp)powerUpList.Find(p => p.GetType().IsSubclassOf(typeof(SpecialSkillPowerUp)));
        }

        #endregion
    }

}
