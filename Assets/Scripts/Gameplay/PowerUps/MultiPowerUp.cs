using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public class MultiPowerUp : MonoBehaviour, IPickable
    {
        [SerializeField]
        List<SkillPowerUp> powerUps;

      
        // Start is called before the first frame update
        void Start()
        {
          
        }

        // Update is called once per frame
        void Update()
        {

        }

  
        bool CanBePoweredUp(PlayerController playerController)
        {
            foreach (IPowerUp powerUp in powerUps)
            {
                if (!(powerUp as MonoBehaviour).GetComponent<IPickable>().CanBePicked(playerController.gameObject))
                    return false;
            }

            return true;
        }

        public void PickUp(GameObject picker)
        {
            // Add all the power ups
            foreach (IPowerUp powerUp in powerUps)
                powerUp.Activate(picker);
        }

        public bool CanBePicked(GameObject picker)
        {
            return CanBePoweredUp(picker.GetComponent<PlayerController>());
        }
    }

}
