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
        public static SettingsManager Instance { get; private set; }

        // The current resolution id
        Resolution resolution;
        //public Resolution Resolution
        //{
        //    get { return resolution; }
        //}

        // The current screen mode
        int screenMode;
        //public int ScreenMode
        //{
        //    get { return screenMode; }
        //}

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                InitResolution();
                InitScreenMode();

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
        void InitResolution()
        {
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                // Set current
                if (Screen.resolutions[i].Equals(Screen.currentResolution))
                    resolution = Screen.resolutions[i];
            }
        }

        void InitScreenMode()
        {
            screenMode = (int)Screen.fullScreenMode;
        }
        #endregion
    }

}
