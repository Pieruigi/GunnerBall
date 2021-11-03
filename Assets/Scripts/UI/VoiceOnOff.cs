using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class VoiceOnOff : MonoBehaviour
    {
        private string msgText = "Press <color=red>V</color> to switch voice {0}";
        private TMP_Text textField;

        private void Awake()
        {
            textField = GetComponent<TMP_Text>();
            
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!VoiceManager.Instance)
                return;

            VoiceManager.Instance.OnVoiceEnabled += VoiceUpdate;
            VoiceManager.Instance.OnVoiceDisabled += VoiceUpdate;

            VoiceUpdate();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            if (!VoiceManager.Instance)
                return;
            VoiceManager.Instance.OnVoiceEnabled -= VoiceUpdate;
            VoiceManager.Instance.OnVoiceDisabled -= VoiceUpdate;
        }

        void VoiceUpdate()
        {
            if (VoiceManager.Instance.VoiceEnabled)
                textField.text = string.Format(msgText, "off");
            else
                textField.text = string.Format(msgText, "on");
        }
    }

}
