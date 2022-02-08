using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum PickableType { SpeedUp, StaminaUp, FreezeUp, FireRateUp, FirePowerUp, FireRangeUp,
                               SpeedDown, StaminaDown, FreezeDown, FireRateDown, FirePowerDown, FireRangeDown }

    public class Pickable : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        PickableType type;
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
            // Try get the player controller from other
            PlayerController playerController = other.GetComponent<PlayerController>();
            
            // It's not a player, so return
            if (!playerController)
                return;

            // If it's not my player returns ( AI is flagged as mine too )
            if (!playerController.photonView.IsMine)
                return;

            // It's the local player or the AI 
            PickableManager.Instance.TryPickUp(gameObject, playerController.photonView.OwnerActorNr);
        }
        #endregion

        #region public methods
        public void PickUp()
        {
            switch (type)
            {
                case PickableType.FirePowerDown:

                    break;
                case PickableType.FirePowerUp:

                    break;
                case PickableType.FireRangeDown:

                    break;
                case PickableType.FireRangeUp:

                    break;
                case PickableType.FireRateDown:

                    break;
                case PickableType.FireRateUp:

                    break;
                case PickableType.FreezeDown:

                    break;
                case PickableType.FreezeUp:

                    break;
                case PickableType.SpeedDown:

                    break;
                case PickableType.SpeedUp:

                    break;
                case PickableType.StaminaDown:

                    break;
                case PickableType.StaminaUp:

                    break;
            }
        }
        #endregion
    }

}
