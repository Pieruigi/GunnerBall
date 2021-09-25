using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    /// <summary>
    /// This class manage application settings, such as video, audio, ecc.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public readonly string MouseSensitivityKey = "MouseSensitivity";

        public static SettingsManager Instance { get; private set; }

        // The current resolution id
        //Resolution resolution;
        //public Resolution Resolution
        //{
        //    get { return resolution; }
        //}

        // The current screen mode
        //int screenMode;
        //public int ScreenMode
        //{
        //    get { return screenMode; }
        //}

        #region controls
        float mouseSensitivityDefault = 5f;
        float mouseSensitivity;
        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
        }
        #endregion

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                //InitResolution();
                //InitScreenMode();
                // Resolution is handled by the engine settings so we don't need
                // to save it

                // Read controls
                ReadControlsSettings();

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region private
        //void InitResolution()
        //{
        //    for (int i = 0; i < Screen.resolutions.Length; i++)
        //    {
        //        // Set current
        //        if (Screen.resolutions[i].Equals(Screen.currentResolution))
        //            resolution = Screen.resolutions[i];
        //    }
        //}

        //void InitScreenMode()
        //{
        //    screenMode = (int)Screen.fullScreenMode;
        //}
        #endregion

        #region private controls
        void ReadControlsSettings()
        {
            // Mouse sensitivity
            mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, mouseSensitivityDefault);
        }

        void WriteControlsSettings()
        {
            PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivity);
        }
        #endregion
    }

}
