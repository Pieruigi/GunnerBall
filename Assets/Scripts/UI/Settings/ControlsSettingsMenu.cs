using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class ControlsSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSelector mouseSensitivityOption;


       
        #region internal fields
        int mouseSensitivityId = -1;
        int oldMouseSensitivityId = -1;
        #endregion

        private void Awake()
        {
            mouseSensitivityOption.OnChange += delegate (int id) 
            {
                // Set the new id
                mouseSensitivityId = id;
                // Get value
                float value = float.Parse(mouseSensitivityOption.GetOption(id));
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

            // Create option list
            List<string> options = new List<string>();
            for(int i=1; i<=10; i++)
            {
                options.Add(i.ToString());
            }

            // Set option list
            mouseSensitivityOption.SetOptions(options);

            // Set current id
            mouseSensitivityId = (int)SettingsManager.Instance.MouseSensitivity-1;
            oldMouseSensitivityId = mouseSensitivityId;
            mouseSensitivityOption.SetCurrentOptionId(mouseSensitivityId);
        }

       
        #endregion
    }

}
