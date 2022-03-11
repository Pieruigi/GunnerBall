using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca.UI
{
    public class ModeStatsDetail : MonoBehaviour
    {
        [SerializeField]
        TMP_Text modeText;

        [SerializeField]
        TMP_Text winText;

        [SerializeField]
        TMP_Text drawText;

        [SerializeField]
        TMP_Text loseText;

        [SerializeField]
        TMP_Text pointsText;

        bool started = false;   
        // Start is called before the first frame update
        void Start()
        {
            started = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            Debug.Log("Started:"+started);
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        
    }

}
