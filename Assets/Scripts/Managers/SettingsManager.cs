using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Zoca
{
    enum SettingType { MasterVolume, MusicVolume, FxVolume, ScreenResolution, ScreenMode }

    /// <summary>
    /// This class manage application settings, such as video, audio, ecc.
    /// NB:
    /// Values stored as float in the registry are not correctly represented 
    /// in the registry editor because of some bug ( but the value is still correct ); 
    /// so we are using string to be able to check for the right value.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public UnityAction OnResolutionChanged;
        public UnityAction OnHideNicknameChanged;

        public readonly string MouseSensitivityKey = "MouseSensitivity";
        public readonly string HideNicknameKey = "HideNickname";

        #region properties
        public static SettingsManager Instance { get; private set; }

        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
        }

        public float MasterVolume
        {
            get { return masterVolume; }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
        }

        public float FXVolume
        {
            get { return fxVolume; }
        }

        public bool HideNickname
        {
            get { return hideNickname; }
        }
        #endregion

        #region private fields

        [SerializeField]
        InputActionAsset inputActionAsset;

        [SerializeField]
        AudioMixer audioMixer;
       
        //
        // Controls
        //
        float mouseSensitivityDefault = 11f;
        float mouseSensitivity;
        
       

        //
        // Audio
        //
        float masterVolume;
        float musicVolume;
        float fxVolume;
       
        string masterVolumeParam = "MasterVolume";
        string musicVolumeParam = "MusicVolume";
        string fxVolumeParam = "FXVolume";

        //
        // Misc
        //
        bool saveEnabled = false;
        bool hideNickname = false;
        bool hideNicknameDefault = false;
        #endregion




        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;


                // Resolution is handled by the engine settings so we don't need
                // to save it


                // Init audio
                InitAudioSettings();

                // Read settings
                InitControlsSettings();

                // Init misc settings
                InitMiscSettings();

                // Input action rebinding if needed
                //InitKeyboardBinding();

             

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
            // Setting mixer ( can't be done in the awake )
            InitMixer();

            saveEnabled = true;
        }

        // Update is called once per frame
        void Update()
        {
        }

        

        #region public setter
        public void SetMouseSensitivity(float value)
        {
            mouseSensitivity = value;

            if (!saveEnabled)
                return;

            PlayerPrefs.SetFloat(MouseSensitivityKey, value);
            PlayerPrefs.Save();
        }

        public void SetMasterVolume(float value)
        {
            masterVolume = value;
            audioMixer.SetFloat(masterVolumeParam, GeneralUtility.LinearToDecibel(value));

            if (!saveEnabled)
                return;

            PlayerPrefs.SetFloat(masterVolumeParam, value);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float value)
        {
            musicVolume = value;
            audioMixer.SetFloat(musicVolumeParam, GeneralUtility.LinearToDecibel(value));

            if (!saveEnabled)
                return;

            PlayerPrefs.SetFloat(musicVolumeParam, value);
            PlayerPrefs.Save();
        }

        public void SetFxVolume(float value)
        {
            fxVolume = value;
            audioMixer.SetFloat(fxVolumeParam, GeneralUtility.LinearToDecibel(value));

            if (!saveEnabled)
                return;

            PlayerPrefs.SetFloat(fxVolumeParam, value);
            PlayerPrefs.Save();
        }

        public void SetHideNickname(bool value)
        {
            hideNickname = value;

            if (!saveEnabled)
                return;

            PlayerPrefs.SetInt(HideNicknameKey, value ? 1 : 0);
            PlayerPrefs.Save();

            OnHideNicknameChanged?.Invoke();
        }
        #endregion

       

        #region private controls
        void InitMixer()
        {
            audioMixer.SetFloat(masterVolumeParam, GeneralUtility.LinearToDecibel(masterVolume));
            audioMixer.SetFloat(musicVolumeParam, GeneralUtility.LinearToDecibel(musicVolume));
            audioMixer.SetFloat(fxVolumeParam, GeneralUtility.LinearToDecibel(fxVolume));
        }

        void InitAudioSettings()
        {
            // Read player prefs and set params
            masterVolume = PlayerPrefs.GetFloat(masterVolumeParam, 0.8f);
            musicVolume = PlayerPrefs.GetFloat(musicVolumeParam, 0.8f);
            fxVolume = PlayerPrefs.GetFloat(fxVolumeParam, 0.8f);
            
        }

        void InitControlsSettings()
        {
            // Mouse sensitivity
            mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, mouseSensitivityDefault);
        }

        void InitMiscSettings()
        {
            // Hide nickname
            hideNickname = PlayerPrefs.GetInt(HideNicknameKey, hideNicknameDefault ? 1 : 0) == 0 ? false : true;
        }

        void InitKeyboardBinding()
        {
            
            InputControlScheme? ics = inputActionAsset.FindControlScheme("XBox Gamepad");
            if (ics == null)
                Debug.LogError("SettingsManager - No input control scheme found: MouseAndKeyboard");
            else
                Debug.Log(ics.Value);
            

            InputActionMap map = inputActionAsset.FindActionMap("PlayerControls");
            if(map == null)
                Debug.LogError("SettingsManager - No action map found: PlayerControls");

            // Get sprint action
            InputAction action = map.FindAction("Sprint");
            Debug.Log("Binding:" + action.bindings[1].ToDisplayString());
            Debug.Log("Binding:" + action.bindings[1].effectivePath);
            Debug.Log("Binding:" + action.bindings[1].path);
            Debug.Log("Binding:" + action.bindings[1].overridePath);
            //Debug.Log("Binding:" + action.);
            int bindingIndex = new List<InputBinding>(action.bindings).FindIndex(b => b.path.StartsWith("<Keyboard>"));
            Debug.Log("Index:" + bindingIndex);
            action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .WithControlsExcluding("XInputController");
                //.Start();

                
               

            //action.bind
        }

        #endregion


        #region utility
        
        public float VolumeToDecibel(float volume)
        {
            float ratio = volume / 10f;
            return Mathf.Lerp(-80, 0, ratio);
        }

        #endregion

    }

}
