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
        float offset = 423.5f;
        float anchoredY = 0;

        private void Awake()
        {
            healthImage = GetComponent<Image>();
            anchoredY = healthImage.rectTransform.anchoredPosition.y;
        }

        // Start is called before the first frame update
        void Start()
        {
            health = PlayerController.Local.Health;
            //healthImage.fillAmount = health / PlayerController.Local.HealthMax;
            //pos.x = offset - offset * health / PlayerController.Local.HealthMax;
            //healthImage.transform.localPosition = pos;
            Vector2 pos = new Vector2(0, anchoredY);
            pos.x = offset * ( health / PlayerController.Local.HealthMax - 1 );
            healthImage.rectTransform.anchoredPosition = pos;
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


                //healthImage.fillAmount = health / PlayerController.Local.HealthMax;

                Vector2 pos = new Vector2(0, anchoredY);
                pos.x = offset * ( health / PlayerController.Local.HealthMax - 1 );
                healthImage.rectTransform.anchoredPosition = pos;
            }
        }
    }

}
