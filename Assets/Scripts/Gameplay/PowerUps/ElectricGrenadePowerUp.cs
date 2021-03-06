using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class ElectricGrenadePowerUp : SpecialSkillPowerUp
    {
        protected override bool DoTryShoot()
        {
            Debug.Log("Shooting grenade");
            //// Get the owner player controller
            //PlayerController playerController = Target.GetComponent<PlayerController>();

            //// Cast a ray from the camera
            //Ray ray = new Ray(playerController.PlayerCamera.transform.position, playerController.PlayerCamera.transform.forward);
            //RaycastHit info;
            //if (Physics.Raycast(ray, out info, 1000))
            //{
            //    // Create the barrier
            //    int team = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(playerController.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
            //    team = team == (int)Team.Blue ? (int)Team.Red : (int)Team.Blue;

            //    LevelManager.Instance.SendEventSpawnElectricGrenade(info.point, Target.transform.rotation, team);
            //    return true;
            //}

            // Get the owner player controller
            PlayerController playerController = Target.GetComponent<PlayerController>();

            // Cast a ray from the camera
            Ray ray = new Ray(playerController.PlayerCamera.transform.position, playerController.PlayerCamera.transform.forward);
            int layer = LayerMask.GetMask(new string[] { Layer.Ground });
            RaycastHit info;
            if (CastRayFromPlayerCamera(out info, layer))
            {
                // Create the barrier
                int team = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(playerController.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
                team = team == (int)Team.Blue ? (int)Team.Red : (int)Team.Blue;

                LevelManager.Instance.SendEventSpawnElectricGrenade(info.point, Target.transform.rotation, team);
                return true;
            }

            // No ground hit, no barrier
            return false;
        }

        

    }

}
