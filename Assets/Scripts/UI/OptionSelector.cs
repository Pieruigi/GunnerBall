using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class OptionSelector : MonoBehaviour
    {
        
        [SerializeField]
        TMP_Text textLabel;

        [SerializeField]
        Button buttonPrev;

        [SerializeField]
        Button buttonNext;

        [SerializeField]
        TMP_Text textValue;

        List<string> options;

        int currentOptionId = -1;

        private void Awake()
        {
            buttonNext.onClick.AddListener(OnMoveNext);
            buttonPrev.onClick.AddListener(OnMovePrev);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetLabel(string label)
        {
            textLabel.text = label;
        }

        public void SetOptions(List<string> options)
        {
            this.options = options;
        }

        public void SetCurrentOptionId(int optionId)
        {
            if (options == null)
                throw new System.Exception("OptionSelector - SetCurrentOptionId() fails; no options found, please call SetOptions() first.");

            currentOptionId = optionId;
            textValue.text = options[currentOptionId];

            // Check buttons
            if (currentOptionId == options.Count - 1)
            {
                // Disable next button
                buttonNext.interactable = false;
            }
            if (currentOptionId == 0)
            {
                // Disable prev button
                buttonPrev.interactable = false;
            }
        }

        #region private
        void OnMoveNext()
        {
            if (currentOptionId < options.Count - 1)
            {
                // Update current id
                currentOptionId++;
                // Show the new value
                textValue.text = options[currentOptionId];

                // Enable prev button
                buttonPrev.interactable = true;
            }
                
            // Check for the end of the option list
            if(currentOptionId == options.Count - 1)
            {
                // Disable next button
                buttonNext.interactable = false;
            }

        }


        void OnMovePrev()
        {
            if (currentOptionId > 0)
            {
                // Update current id
                currentOptionId--;
                // Show the new value
                textValue.text = options[currentOptionId];

                // Enable next button
                buttonNext.interactable = true;
            }

            // Check for the start of the option list
            if (currentOptionId == 0)
            {
                // Disable prev button
                buttonPrev.interactable = false;
            }
        }
        #endregion
    }

}
