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
            return true;
        }
    }

}
