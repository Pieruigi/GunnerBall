using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class EndGame : MonoBehaviour
    {
        [SerializeField]
        Text winnerText;

        [SerializeField]
        Text blueScoreText;

        [SerializeField]
        Text redScoreText;

       
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
            // Get scores
            int blueScore = Match.Instance.BlueTeamScore;
            int redScore = Match.Instance.RedTeamScore;

            // Set winner text
            if (blueScore == redScore)
            {
                winnerString = "Draw";
            }
            else
            {
                if (blueScore > redScore)
                {
                    winnerString = "Blue Team Wins";
                }
                else
                {
                    winnerString = "Red Team Wins";
                }
            }

            // Fill text fields
            winnerText.text = winnerString;
            blueScoreText.text = blueScore.ToString();
            redScoreText.text = redScore.ToString();
        }

       
    }

}
