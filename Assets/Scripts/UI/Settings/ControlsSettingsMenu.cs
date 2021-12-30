using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class ControlsSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSlider mouseSensitivityOption;


        private void Awake()
        {
            mouseSensitivityOption.OnChange += delegate (float value) 
            {
                // Set new value
                SettingsManager.Instance.SetMouseSensitivity(value);
            };

        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (!SettingsManager.Instance)
                return;

            // Init UI
            InitMouseSensitivityOption();
            
        }

        private void OnDisable()
        {
           
        }

     

        #region private

       

        void InitMouseSensitivityOption()
        {
            

            // Set label
            mouseSensitivityOption.SetLabel("Mouse Sensitivity");

            
            mouseSensitivityOption.InitValue(SettingsManager.Instance.MouseSensitivity);
        }

       
        #endregion
    }

}
