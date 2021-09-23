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

        #region text fields
        string fullScreenExclusiveModeOption = "FullScreenExclusive";
        string fullScreenModeOption = "FullScreen";
        string fullScreenWindowedModeOption = "FullScreenWindowed";
        string windowedModeOption = "Windowed";
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Init UI
            InitResolutionOption();
            InitScreenModeOption();
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
            foreach(Resolution res in resList)
            {
                options.Add(string.Format("{0}x{1}x{2}", res.width, res.height, res.refreshRate));
            }
            resolutionOption.SetOptions(options);
            // Set current option
            resolutionOption.SetCurrentOptionId(resList.IndexOf(SettingsManager.Instance.Resolution));
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
