using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Interfaces
{
    public interface IHittable
    {
       
        void Hit(GameObject owner, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection, float hitPower);


    }

}
