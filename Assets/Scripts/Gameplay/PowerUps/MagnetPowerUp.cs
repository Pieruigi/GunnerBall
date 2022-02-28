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
            int mask = LayerMask.GetMask(new string[] { Layer.Wall, Layer.Top });
            if(CastRayFromPlayerCamera(out info, mask))
            {
                LevelManager.Instance.SendEventSpawnMagnet(info.point, Quaternion.identity);
                return true;
            }

            return false;
        }
    }

}
