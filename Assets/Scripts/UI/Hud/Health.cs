using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class Health : MonoBehaviour
    {
        Image healthImage;

        float health;
        float speed = 100;

        private void Awake()
        {
            healthImage = GetComponent<Image>();
        }

        // Start is called before the first frame update
        void Start()
        {
            health = PlayerController.Local.Health;
            healthImage.fillAmount = health / PlayerController.Local.HealthMax;
        }

        // Update is called once per frame
        void Update()
        {
            if(PlayerController.Local.Health != health)
            {
                if(health < PlayerController.Local.Health)
                {
                    health = Mathf.Min(health + Time.deltaTime * speed, PlayerController.Local.Health);
                }
                else
                {
                    health = Mathf.Max(health - Time.deltaTime * speed, PlayerController.Local.Health);
                }


                healthImage.fillAmount = health / PlayerController.Local.HealthMax;
            }
        }
    }

}
