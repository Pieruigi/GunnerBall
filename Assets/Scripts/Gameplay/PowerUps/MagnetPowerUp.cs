using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class MagnetPowerUp : SpecialSkillPowerUp
    {
        protected override bool DoTryShoot()
        {
            RaycastHit info;
            if(CastRayFromPlayerCamera(out info))
            {
                LevelManager.Instance.SendEventSpawnMagnet(info.point, Quaternion.identity);
                return true;
            }

            return false;
        }
    }

}
