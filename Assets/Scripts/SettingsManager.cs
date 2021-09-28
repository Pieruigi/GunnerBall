using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        #endregion

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
    }

}
