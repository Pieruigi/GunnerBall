using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSelector masterVolumeOption;

        [SerializeField]
        OptionSelector musicVolumeOption;

        [SerializeField]
        OptionSelector fxVolumeOption;

        private void Awake()
        {
            masterVolumeOption.OnChange += delegate (int id)
            {
                SettingsManager.Instance.SetMasterVolume(int.Parse(masterVolumeOption.GetOption(id)));
            };
            musicVolumeOption.OnChange += delegate (int id)
            {
                SettingsManager.Instance.SetMusicVolume(int.Parse(musicVolumeOption.GetOption(id)));
            };
            fxVolumeOption.OnChange += delegate (int id)
            {
                SettingsManager.Instance.SetFxVolume(int.Parse(fxVolumeOption.GetOption(id)));
            };
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
            // Init master
            // Label
            masterVolumeOption.SetLabel("Master Volume");
            // Option list
            for (int i = 0; i<11; i++)
            {
                masterVolumeOption.SetOptions(new List<string>(new string[]{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" } ));
            }
            // Selected option
            masterVolumeOption.SetCurrentOptionId(SettingsManager.Instance.MasterVolume);

            // Init music
            // Label
            musicVolumeOption.SetLabel("Music Volume");
            // Option list
            for (int i = 0; i < 11; i++)
            {
                musicVolumeOption.SetOptions(new List<string>(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }));
            }
            // Selected option
            musicVolumeOption.SetCurrentOptionId(SettingsManager.Instance.MusicVolume);

            // Init fx
            // Label
            fxVolumeOption.SetLabel("FX Volume");
            // Option list
            for (int i = 0; i < 11; i++)
            {
                fxVolumeOption.SetOptions(new List<string>(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }));
            }
            // Selected option
            fxVolumeOption.SetCurrentOptionId(SettingsManager.Instance.FXVolume);

        }
    }

}
