using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class EndGame : MonoBehaviour
    {
        [SerializeField]
        TMP_Text winnerText;

        [SerializeField]
        TMP_Text blueScoreText;

        [SerializeField]
        TMP_Text redScoreText;

       
        string winnerString;

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (!Match.Instance)
                return;

            // Get scores
            int blueScore = Match.Instance.BlueTeamScore;
            int redScore = Match.Instance.RedTeamScore;

            // Set winner text
            if (blueScore == redScore)
            {
                winnerString = "You Draw";
            }
            else
            {
                if (blueScore > redScore)
                {
                    winnerString = "<color=#00C8FF>Blue</color> Wins";
                }
                else
                {
                    winnerString = "<color=red>Red</color> Wins";
                }
            }

            // Fill text fields
            winnerText.text = winnerString;
            blueScoreText.text = blueScore.ToString();
            redScoreText.text = redScore.ToString();
        }

       
    }

}
