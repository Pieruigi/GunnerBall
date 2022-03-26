
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
            //masterVolumeOption.OnChange += HandleOnMasterVolumeChange;
            musicVolumeOption.OnChange += HandleOnMusicVolumeChange;
            fxVolumeOption.OnChange += HandleOnFxVolumeChange;
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
            //masterVolumeOption.SetLabel("MasterVolume");
            //masterVolumeOption.InitValue(SettingsManager.Instance.MasterVolume);

            musicVolumeOption.SetLabel("MusicVolume");
            musicVolumeOption.InitValue(SettingsManager.Instance.MusicVolume);
            
            fxVolumeOption.SetLabel("FxVolume");
            fxVolumeOption.InitValue(SettingsManager.Instance.FXVolume);
        }

        void HandleOnMasterVolumeChange(float value)
        {
            SettingsManager.Instance.SetMasterVolume(value);
        }

        void HandleOnMusicVolumeChange(float value)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }

        void HandleOnFxVolumeChange(float value)
        {
            SettingsManager.Instance.SetFxVolume(value);
        }
    }

}
