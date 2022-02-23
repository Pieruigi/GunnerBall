using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zoca.Interfaces;

namespace Zoca
{
    public class MultiPowerUp : MonoBehaviour, IPickable
    {
        public event UnityAction<IPickable, GameObject> OnPicked;

       
        [SerializeField]
        List<SkillPowerUp> powerUps;

        [SerializeField]
        GameObject endParticle;


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
            endParticle.transform.parent = null;
            endParticle.GetComponent<ParticleSystem>().Play();
            Destroy(endParticle, 10);

            if (!picker)
                return;

            if (PlayerController.LocalPlayer == picker || PhotonNetwork.OfflineMode)
            {
                // Add all the power ups
                foreach (IPowerUp powerUp in powerUps)
                    powerUp.Activate(picker);


            }
            

            

            

            OnPicked?.Invoke(this, picker);

        }

        public bool CanBePicked(GameObject picker)
        {
            return CanBePoweredUp(picker.GetComponent<PlayerController>());
        }
    }

}
