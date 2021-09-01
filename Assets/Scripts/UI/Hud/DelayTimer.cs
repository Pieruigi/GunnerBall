using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class DelayTimer : MonoBehaviour
    {
        [SerializeField]
        Text timerText;

        bool loop = false;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            if(Match.Instance.State != MatchState.Starting)
            {
                loop = false;
                
                // Hide text
                timerText.gameObject.SetActive(false);
            }
            else
            {
                loop = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!loop)
                return;

            if(Match.Instance.State != MatchState.Starting)
            {
                loop = false;
                // Hide text
                timerText.gameObject.SetActive(false);
                return;
            }

            // Set the timer
            int timer = (int)(Constants.StartDelay - Mathf.Max((float)PhotonNetwork.Time - Match.Instance.StartTime, 0));

            timerText.text = timer.ToString();

        }
    }

}
