using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Interfaces
{
    public interface IDamageable
    {
        void ApplyDamage(float damage);

        bool IsDestroyed();

        void Heal(float value);
        
    }

}
