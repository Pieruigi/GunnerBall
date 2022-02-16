using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class BarrierPowerUp : SpecialSkillPowerUp
    {
        protected override bool DoTryShoot()
        {
            
            Debug.Log("Shooting barrier");
            // Get the owner player controller
            PlayerController playerController = Target.GetComponent<PlayerController>();

            // Cast a ray from the camera
            Ray ray = new Ray(playerController.PlayerCamera.transform.position, playerController.PlayerCamera.transform.forward);
            int mask = LayerMask.GetMask(new string[] { Layer.Ground });
            RaycastHit info;
            if(Physics.Raycast(ray, out info, 1000, mask))
            {
                // Create the barrier
                LevelManager.Instance.SpawnBarrier(info.point, Target.transform.rotation);
                return true;
            }
            
            // No ground hit, no barrier
            return false;
            
        }
    }

}
