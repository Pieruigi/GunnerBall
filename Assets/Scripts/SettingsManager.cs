using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Zoca
{
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

        public readonly string MouseSensitivityKey = "MouseSensitivity";

        public static SettingsManager Instance { get; private set; }

        [SerializeField]
        InputActionAsset inputActionAsset;

        [SerializeField]
        AudioMixer audioMixer;
       
        #region controls
        float mouseSensitivityDefault = 5f;
        float mouseSensitivity;
        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
        }
        #endregion

        #region audio
        int masterVolume;
        public int MasterVolume
        {
            get { return masterVolume; }
        }
        int musicVolume;
        public int MusicVolume
        {
            get { return musicVolume; }
        }
        int fxVolume;
        public int FXVolume
        {
            get { return fxVolume; }
        }

        string masterVolumeParam = "MasterVolume";
        string musicVolumeParam = "MusicVolume";
        string fxVolumeParam = "FXVolume";
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

                // Input action rebinding if needed
                InitKeyboardBinding();

             

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
            // Setting mixer
            audioMixer.SetFloat(masterVolumeParam, VolumeToDecibel(masterVolume));
            audioMixer.SetFloat(musicVolumeParam, VolumeToDecibel(musicVolume));
            audioMixer.SetFloat(fxVolumeParam, VolumeToDecibel(fxVolume));
        }

        // Update is called once per frame
        void Update()
        {

        }

        

        #region public setter
        public void SetMouseSensitivity(float value)
        {
            mouseSensitivity = value;

            PlayerPrefs.SetString(MouseSensitivityKey, value.ToString());
            PlayerPrefs.Save();
        }

        public void SetMasterVolume(int value)
        {
            masterVolume = value;
            audioMixer.SetFloat(masterVolumeParam, VolumeToDecibel(value));
            
            PlayerPrefs.SetString(masterVolumeParam, value.ToString());
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(int value)
        {
            musicVolume = value;
            audioMixer.SetFloat(musicVolumeParam, VolumeToDecibel(value));

            PlayerPrefs.SetString(musicVolumeParam, value.ToString());
            PlayerPrefs.Save();
        }

        public void SetFxVolume(int value)
        {
            fxVolume = value;
            audioMixer.SetFloat(fxVolumeParam, VolumeToDecibel(value));

            PlayerPrefs.SetString(fxVolumeParam, value.ToString());
            PlayerPrefs.Save();
        }

        #endregion

       

        #region private controls

        void InitAudioSettings()
        {
            // Read player prefs and set params
            masterVolume = int.Parse(PlayerPrefs.GetString(masterVolumeParam, "10"));
            musicVolume = int.Parse(PlayerPrefs.GetString(musicVolumeParam, "10"));
            fxVolume = int.Parse(PlayerPrefs.GetString(fxVolumeParam, "10"));
            
        }

        void InitControlsSettings()
        {
            // Mouse sensitivity
            mouseSensitivity = float.Parse(PlayerPrefs.GetString(MouseSensitivityKey, mouseSensitivityDefault.ToString()));
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
