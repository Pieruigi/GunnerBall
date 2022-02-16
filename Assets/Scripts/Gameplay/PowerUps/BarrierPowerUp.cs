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
            int mask = LayerMask.GetMask(new string[] { Layer.Ground, Layer.Wall });
            RaycastHit info;
            if(Physics.Raycast(ray, out info, 1000, mask))
            {
                
                if(info.transform.gameObject.layer == LayerMask.NameToLayer(Layer.Wall))
                {
                    // We hit the wall, so we must check for the ground to create the barrier
                    ray = new Ray(info.point, Vector3.down);
                    mask = LayerMask.GetMask(new string[] { Layer.Ground });
                    if(Physics.Raycast(ray, out info, 1000, mask))
                    {
                        LevelManager.Instance.SpawnBarrier(info.point, Target.transform.rotation);
                    }
                }
                else
                {
                    LevelManager.Instance.SpawnBarrier(info.point, Target.transform.rotation);
                }

            }

            
            
            return true;
        }
    }

}
