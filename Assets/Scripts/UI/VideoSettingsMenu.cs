using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class VideoSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSelector resolutionOption;

        [SerializeField]
        OptionSelector screenModeOption;

        [SerializeField]
        OptionSelector refreshRateOption;

        #region text fields
        string fullScreenExclusiveModeOption = "FullScreenExclusive";
        string fullScreenModeOption = "FullScreen";
        string fullScreenWindowedModeOption = "FullScreenWindowed";
        string windowedModeOption = "Windowed";
        #endregion

        #region internal fields
        string resolutionFormat = "{0} X {1}";
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Init UI
            InitResolutionOption();
            InitScreenModeOption();
            InitRefreshRateOption();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        #region private
        void InitResolutionOption()
        {
            // Set label
            resolutionOption.SetLabel("Resolution");

            // Get all the resolutions as list
            List<Resolution> resList = new List<Resolution>(Screen.resolutions);
            // Set options
            List<string> options = new List<string>();
           
            int currentId = -1;
            foreach(Resolution res in resList)
            {
                // We split resolution from refresh rate
                if(options.Find(r => r.Equals(string.Format(resolutionFormat, res.width, res.height))) == null)
                {
                    options.Add(string.Format(resolutionFormat, res.width, res.height));
                    
                    // We check for the current resolution
                    if (res.width == Screen.currentResolution.width && 
                        res.height == Screen.currentResolution.height &&
                        currentId < 0)
                    {
                        currentId = options.Count - 1;
                    }
                }

                
                    
            }
            resolutionOption.SetOptions(options);
            // Set current option
            resolutionOption.SetCurrentOptionId(currentId);
        }

        void InitRefreshRateOption()
        {
            // Label
            refreshRateOption.SetLabel("Refresh Rate");

            // Get resolution list
            List<string> options = new List<string>();
            int currentId = -1;
            foreach(Resolution res in Screen.resolutions)
            {
                if(options.Find(r=>r.Equals(res.refreshRate.ToString())) == null)
                {
                    // Add new option
                    options.Add(res.refreshRate.ToString());

                    // Check for the current refresh rate
                    if (res.refreshRate == Screen.currentResolution.refreshRate &&
                        currentId < 0)
                        currentId = options.Count - 1;
                }
            }

            refreshRateOption.SetOptions(options);
            refreshRateOption.SetCurrentOptionId(currentId);
        }

        void InitScreenModeOption()
        {
            // Set label
            screenModeOption.SetLabel("ScreenMode");

            // Set the options
            List<string> options = new List<string>();
            options.Add(fullScreenExclusiveModeOption);
            options.Add(fullScreenModeOption);
            options.Add(fullScreenWindowedModeOption);
            options.Add(windowedModeOption);
            
            screenModeOption.SetOptions(options);

            // Set the current mode
            screenModeOption.SetCurrentOptionId((int)Screen.fullScreenMode);
        }
        #endregion
    }

}
