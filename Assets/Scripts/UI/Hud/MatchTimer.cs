using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    
    public class MatchTimer : MonoBehaviour
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
            int timer = (int)Mathf.Max(Match.Instance.Length - Match.Instance.TimeElapsed, 0);
            
            timerText.text = timer.ToString();
        }
    }

}
