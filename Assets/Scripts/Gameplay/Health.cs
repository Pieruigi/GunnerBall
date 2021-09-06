using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField]
        float healthMax = 150;

        [SerializeField]
        float health = 100;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ApplyDamage(float damage)
        {
            health = Mathf.Max(0, health - damage);
        }

        public bool IsDestroyed()
        {
            return health == 0;
        }

        public void Heal(float value)
        {
            health = Mathf.Min(healthMax, health + value);
        }
    }

}
