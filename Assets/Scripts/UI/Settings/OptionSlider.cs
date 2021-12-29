using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class OptionSlider : MonoBehaviour
    {
        public UnityAction<float> OnChange;

        #region private fields
        [SerializeField]
        TMP_Text label;

        [SerializeField]
        Slider slider;

        bool onChangeEnabled;
        #endregion

        #region private methods
        private void Awake()
        {
            slider.onValueChanged.AddListener(delegate (float value) { if (onChangeEnabled) OnChange?.Invoke(value); });
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region public methods
        public void SetLabel(string text)
        {
            label.text = text;
        }

        public void InitValue(float value)
        {
            onChangeEnabled = false;
            slider.value = value;
            onChangeEnabled = true;
        }


        #endregion
    }

}
