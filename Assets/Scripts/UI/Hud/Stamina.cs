using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class Stamina : MonoBehaviour
    {
        Image staminaImage;

       
        private void Awake()
        {
            staminaImage = GetComponent<Image>();
        }

        // Start is called before the first frame update
        void Start()
        {
            staminaImage.fillAmount = PlayerController.Local.Stamina / PlayerController.Local.StaminaMax;
        }

        // Update is called once per frame
        void Update()
        {
            staminaImage.fillAmount = PlayerController.Local.Stamina / PlayerController.Local.StaminaMax;
        }
    }

}
