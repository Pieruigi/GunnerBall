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
        OptionType optionType;

        [SerializeField]
        TMP_Text textLabel;

        [SerializeField]
        Image buttonPrev;

        [SerializeField]
        Image buttonNext;

        [SerializeField]
        TMP_Text textValue;

        Option option;

        private void Awake()
        {
            option = new Option(optionType);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
