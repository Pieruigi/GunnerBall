using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    

    public class PowerUp : MonoBehaviour, IPickable
    {
        #region properties
        public float Buff
        {
            get { return buff; }
        }

        public float Time
        {
            get { return time; }
        }

        public Skill Skill
        {
            get { return skill; }
        }
        #endregion

        #region private fields
        [SerializeField]
        float buff = 1.2f;

        [SerializeField]
        float time = 20;

        [SerializeField]
        Skill skill = Skill.Speed;

        bool picking = false; // A local flag to avoid the player picks the object twice
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
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
            if (playerController.GetComponent<PowerUpManager>().CanBePoweredUp())
            {
                picking = true;
                PickableManager.Instance.TryPickUp(gameObject, playerController.photonView.OwnerActorNr);
            }
                
        }
        #endregion

        #region IPickable implementation
        
        public void PickUp(GameObject picker)
        {
            //throw new System.NotImplementedException();
            picker.GetComponent<PowerUpManager>().PowerUp(this);
        }

       
        #endregion
    }

}
