using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public class MultiPowerUp : MonoBehaviour, IPickable
    {
        [SerializeField]
        List<PowerUp> powerUps;

        bool picking = false;
        // Start is called before the first frame update
        void Start()
        {
            Match.Instance.OnStateChanged += delegate { picking = false; };

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (Match.Instance.State == (int)MatchState.Goaled)
            {
                return;
            }


            if (picking)
                return;

            // Try get the player controller from other
            PlayerController playerController = other.GetComponent<PlayerController>();

            // It's not a player, so return
            if (!playerController)
                return;

            // If it's not my player returns ( AI is flagged as mine too )
            if (!playerController.photonView.IsMine)
                return;

            // It's the local player or the AI 
            if (CanBePoweredUp(playerController))
            {
                picking = true;
                PickableManager.Instance.TryPickUp(gameObject, playerController.photonView.OwnerActorNr);
            }

        }

        bool CanBePoweredUp(PlayerController playerController)
        {
            foreach (PowerUp powerUp in powerUps)
            {
                if (!playerController.GetComponent<PowerUpManager>().CanBePoweredUp(powerUp))
                    return false;
            }

            return true;
        }

        public void PickUp(GameObject picker)
        {
            // Add all the power ups
            foreach (PowerUp powerUp in powerUps)
                powerUp.PickUp(picker);
        }
    }

}
