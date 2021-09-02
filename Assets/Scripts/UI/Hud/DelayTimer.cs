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

      
        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
          
        }

        // Update is called once per frame
        void Update()
        {
                        


            // Set the timer
            int timer = (int)( Mathf.Max(Match.Instance.TargetTime - (float)PhotonNetwork.Time, 0));

            timerText.text = timer.ToString();

        }

       
    }

}
