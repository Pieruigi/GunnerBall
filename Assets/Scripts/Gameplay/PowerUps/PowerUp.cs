using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public abstract class PowerUp : MonoBehaviour, IPowerUp
    {
        #region properties
        public GameObject Target
        {
            get { return target; }
        }
        #endregion
        #region private fields
        GameObject target;

        #endregion
        protected virtual void Awake()
        {

        }

        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

       
        public virtual void Activate(GameObject target)
        {
            this.target = target;

            // Move the powerup object out of the pickable
            transform.parent = null;
            // Add to the powerup manager
            target.GetComponent<PowerUpManager>().Add(this);

        }

        public virtual void Deactivate(GameObject target)
        {
            // Remove from power up manager
            target.GetComponent<PowerUpManager>().Remove(this);

            // Destroy
            Destroy(gameObject);
        }
    }

}
