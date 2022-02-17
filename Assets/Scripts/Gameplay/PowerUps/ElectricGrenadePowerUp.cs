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
            // Get the owner player controller
            PlayerController playerController = Target.GetComponent<PlayerController>();

            // Cast a ray from the camera
            Ray ray = new Ray(playerController.PlayerCamera.transform.position, playerController.PlayerCamera.transform.forward);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 1000))
            {
                // Create the barrier
                int team = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(playerController.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
                StartCoroutine(Spawn(info.point, Target.transform.rotation, team));
                return true;
            }

            // No ground hit, no barrier
            return false;
        }

        IEnumerator Spawn(Vector3 position, Quaternion rotation, int targetTeam)
        {
            yield return new WaitForSeconds(0.5f);

            LevelManager.Instance.SpawnElectricGrenade(position, rotation, targetTeam);
        }

    }

}
