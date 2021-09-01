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
            if (Match.Instance.State != MatchState.Starting && Match.Instance.State != MatchState.Paused)
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
            

            if(Match.Instance.State != MatchState.Starting && Match.Instance.State != MatchState.Paused)
            {
                loop = false;
                // Hide text
                timerText.gameObject.SetActive(false);
            }
            else
            {
                loop = true;

                timerText.gameObject.SetActive(true);
            }

            if (!loop)
                return;

            // Set the timer
            float delay = Match.Instance.State == MatchState.Starting ? Constants.StartDelay : Constants.PauseDelay;
            float start = Match.Instance.State == MatchState.Starting ? Match.Instance.StartTime : Match.Instance.PauseTime;
            int timer = (int)( delay - Mathf.Max((float)PhotonNetwork.Time - start, 0));

            timerText.text = timer.ToString();

        }
    }

}
