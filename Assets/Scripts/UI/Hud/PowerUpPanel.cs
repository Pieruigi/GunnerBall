using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class PowerUpPanel : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        GameObject powerUpTemplate;

        [SerializeField]
        Transform container;

        PowerUpManager pum;
        List<GameObject> powerUps; // List of templates
        #endregion

        #region private methods
        private void Awake()
        {
            // Move the template out of container
            powerUpTemplate.transform.parent = container.parent;
            // Deactivate the template
            powerUpTemplate.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Get the local player power up manager
            pum = PlayerController.Local.GetComponent<PowerUpManager>();

            // Set handles
            //pum.OnPowerUpActivated += HandleOnPowerUpActivated;
            //pum.OnPowerUpDeactivated += HandleOnPowerUpDeactivated;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void HandleOnPowerUpActivated(Skill skill)
        {
            //GameObject o = powerUps.Find(t => t.GetComponent<PowerUpTemplate>().Skill == skill);
            //if (!o)
            //{
            //    // Doesn't exist, create it
            //    o = GameObject.Instantiate(powerUpTemplate, container);
                
            //}

            //// Init template
            //o.GetComponent<PowerUpTemplate>().Init(skill, pum.GetPowerUpTime(skill), pum.GetPowerUpRemainingTime(skill));

        }

        void HandleOnPowerUpDeactivated(Skill skill)
        {

        }

        
        #endregion
    }

}
