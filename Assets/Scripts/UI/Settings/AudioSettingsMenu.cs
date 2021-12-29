
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSlider masterVolumeOption;

        [SerializeField]
        OptionSlider musicVolumeOption;

        [SerializeField]
        OptionSlider fxVolumeOption;

        private void Awake()
        {
            masterVolumeOption.OnChange += HandleOnMasterVolumeChange;
            //masterVolumeOption.OnChange += HandleOnMasterVolumeChange;
            //masterVolumeOption.OnChange += HandleOnMasterVolumeChange;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            if (!SettingsManager.Instance)
                return;

            // Init options
            InitOptions();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void InitOptions()
        {
            masterVolumeOption.SetLabel("MasterVolume");
            masterVolumeOption.InitValue(SettingsManager.Instance.MasterVolume);

        }

        void HandleOnMasterVolumeChange(float value)
        {

        }
    }

}
