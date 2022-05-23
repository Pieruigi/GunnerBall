using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        string resolutionFormat = "{0} X {1}";
        #endregion

        #region internal fields
        int resolutionId = -1;
        int oldResolutionId = -1;
        int screenModeId = -1;
        int oldScreenModeId = -1;
        int refreshRateId = -1;
        int oldRefreshRateId = -1;

        #endregion

        private void Awake()
        {
            resolutionOption.OnChange += delegate(int id) { resolutionId = id; ApplyResolution(); };
            refreshRateOption.OnChange += delegate (int id) { refreshRateId = id; ApplyResolution(); };
            screenModeOption.OnChange += delegate (int id) { screenModeId = id; ApplyResolution(); };
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
            // Init UI
            InitResolutionOption();
            InitScreenModeOption();
            InitRefreshRateOption();
        }

        private void OnDisable()
        {
           
        }

     

        #region private

        void ApplyResolution()
        {
            // Get widht and height
            string option = resolutionOption.GetOption(resolutionId);
            int width = int.Parse(option.Split('X')[0].Trim());
            int height = int.Parse(option.Split('X')[1].Trim());
            int rr = int.Parse(refreshRateOption.GetOption(refreshRateId));

            // Set the new resolution
            Screen.SetResolution(width, height, (FullScreenMode)screenModeId, rr);

            SettingsManager.Instance.OnResolutionChanged?.Invoke();
        }

        void InitResolutionOption()
        {
            // Set label
            resolutionOption.SetLabel("Resolution");

            // Get all the resolutions as list
            List<Resolution> resList = new List<Resolution>(Screen.resolutions);
            // Set options
            List<string> options = new List<string>();

            //int currentId = -1;
            resolutionId = -1;
            foreach(Resolution res in resList)
            {
                // We split resolution from refresh rate
                if(options.Find(r => r.Equals(string.Format(resolutionFormat, res.width, res.height))) == null)
                {
                    options.Add(string.Format(resolutionFormat, res.width, res.height));
                    
                    // We check for the current resolution
                    if (res.width == Screen.width && 
                        res.height == Screen.height &&
                        resolutionId < 0)
                    {
                        resolutionId = options.Count - 1;
                        oldResolutionId = resolutionId;
                    }
                }

                
                    
            }
            resolutionOption.SetOptions(options);
            // Set current option
            resolutionOption.SetCurrentOptionId(resolutionId);
        }

        void InitRefreshRateOption()
        {
            // Label
            refreshRateOption.SetLabel("Refresh Rate");

            // Get resolution list
            List<string> options = new List<string>();
            //int currentId = -1;
            refreshRateId = -1;
            int delta = 0;
            foreach(Resolution res in Screen.resolutions)
            {
                if(options.Find(r=>r.Equals(res.refreshRate.ToString())) == null)
                {
                    // Add new option
                    options.Add(res.refreshRate.ToString());

                    Debug.Log("Adding new refresh rate:" + res.refreshRate.ToString());

                    // Check for the current refresh rate
                    Debug.Log("res.refreshRate:" + res.refreshRate);
                    Debug.Log("cur.refreshRate:" + Screen.currentResolution.refreshRate);
                    Debug.Log("refreshRateId:" + refreshRateId);
                    
                    if(refreshRateId < 0)
                    {
                        // Add the first refresh rate element
                        refreshRateId = 0;
                        delta = Mathf.Abs(int.Parse(options[refreshRateId]) - Screen.currentResolution.refreshRate);
                        oldRefreshRateId = refreshRateId;
                    }
                    else
                    {
                        int currentId = options.Count - 1;
                        int newDelta = Mathf.Abs(int.Parse(options[currentId]) - Screen.currentResolution.refreshRate);
                        if(newDelta < delta)
                        {
                            delta = newDelta;
                            refreshRateId = currentId;
                            oldRefreshRateId = refreshRateId;
                        }
                    }

                    //if (res.refreshRate == Screen.currentResolution.refreshRate && refreshRateId < 0)
                    //{
                    //    Debug.Log("Setting refreshrate id");
                    //    refreshRateId = options.Count - 1;
                    //    oldRefreshRateId = refreshRateId;
                    //}
                        
                }
            }
            Debug.Log("setting refreshRateId:" + refreshRateId);
            refreshRateOption.SetOptions(options);
            refreshRateOption.SetCurrentOptionId(refreshRateId);
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
            screenModeId = (int)Screen.fullScreenMode;
            oldScreenModeId = screenModeId;
            screenModeOption.SetCurrentOptionId((int)Screen.fullScreenMode);
        }
        #endregion
    }

}
