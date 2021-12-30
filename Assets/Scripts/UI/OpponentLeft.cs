using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca.UI
{
    public class OpponentLeft : MonoBehaviour
    {
        [SerializeField]
        TMP_Text messageText;

        [SerializeField]
        GameObject panel;

        string leftTextFormat = "{0} left the match";
        float elapsed = 0;
        float time = 5;

        // Start is called before the first frame update
        void Start()
        {
            panel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(elapsed>0)
            {
                elapsed -= Time.deltaTime;
                if (elapsed <= 0)
                    panel.SetActive(false);
            }
        }

        public void Show(string playerName)
        {
            messageText.text = string.Format(leftTextFormat, playerName);
            panel.SetActive(true);
            elapsed = time;
        }
    }

}
