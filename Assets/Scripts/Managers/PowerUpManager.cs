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
        public UnityAction<IPowerUp> OnPowerUpActivated;
        public UnityAction<IPowerUp> OnPowerUpDeactivated;
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
            if (!playerController.photonView.IsMine)
                return;

            if(Match.Instance.State == (int)MatchState.Goaled)
            {
                if (powerUpList.Count > 0)
                    RemoveAll();
            }
                
        }

        void RemoveAll()
        {
            int count = powerUpList.Count;
            Debug.Log("PowerUpCount:" + powerUpList.Count);
            for (int i = 0; i < count; i++)
            //foreach(IPowerUp pUp in powerUpList)
            {
                IPowerUp pUp = powerUpList[0];
                pUp.Deactivate(gameObject);

                OnPowerUpDeactivated?.Invoke(pUp);
            }

            powerUpList.Clear();
        }


        #endregion

        #region public methods

        public void Add(IPowerUp powerUp)
        {
            powerUpList.Add(powerUp);

            OnPowerUpActivated?.Invoke(powerUp);
        }

        public void Remove(IPowerUp powerUp)
        {
            powerUpList.Remove(powerUp);

            OnPowerUpDeactivated?.Invoke(powerUp);
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
