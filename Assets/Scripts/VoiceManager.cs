using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Zoca
{
    public class VoiceManager : MonoBehaviour
    {
        public UnityAction OnVoiceEnabled;
        public UnityAction OnVoiceDisabled;

        public static VoiceManager Instance { get;  private set; }

        public bool VoiceEnabled
        {
            get { return recorder.TransmitEnabled; }
        }

        #region private fields
        Recorder recorder;
        #endregion

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                recorder = GetComponent<Recorder>();
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

        #region public
        public void SwitchVoiceOnOff()
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;

            if (recorder.TransmitEnabled)
                OnVoiceEnabled?.Invoke();
            else
                OnVoiceDisabled?.Invoke();


        }
        #endregion
    }

}
