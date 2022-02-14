using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public class PickUpTrigger : MonoBehaviour
    {
        [SerializeField]
        GameObject target;

        bool picking = false; // A local flag to avoid the player picks the object twice

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
            if (target.GetComponent<IPickable>().CanBePicked(other.gameObject))
            {
                picking = true;
                PickableManager.Instance.TryPickUp(target, playerController.photonView.OwnerActorNr);
            }

        }
    }

}
