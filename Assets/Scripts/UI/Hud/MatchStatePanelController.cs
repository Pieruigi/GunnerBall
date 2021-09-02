using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class MatchStatePanelController : MonoBehaviour
    {
        [SerializeField]
        List<MatchState> activationStates;

        GameObject panel;

        // Start is called before the first frame update
        void Start()
        {
            // Get panel
            panel = transform.GetChild(0).gameObject;

            Match.Instance.OnStateChanged += CheckState;

            CheckState();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void CheckState()
        {
            if (activationStates.Contains((MatchState)Match.Instance.State))
            {
                panel.SetActive(true);
                
            }
            else
            {

                panel.SetActive(false);
            }
        }
    }

}
