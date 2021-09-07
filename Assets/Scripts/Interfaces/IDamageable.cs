using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Interfaces
{
    public interface IDamageable
    {
        /// <summary>
        /// Return the actual amount of damage applied; if the damage is more than
        /// the remainig health then the difference is returned
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        float ApplyDamage(float damage);

        bool IsDestroyed();

        void Heal(float value);

        float GetHealth();
        
        
    }

}
