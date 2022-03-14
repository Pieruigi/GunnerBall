using Photon.Pun;
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

        [SerializeField]
        TMP_Text pointsMessageText;

      
        string winnerString;
        string pointsMessageOnlineFormat = "You Gained <color=green>{0}</color> Points";
        string pointsMessageOfflineFormat = "No Points In Offline Mode";

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
            if (!Match.Instance || Match.Instance.State != (int)MatchState.Completed)
                return;

            Crosshair.Instance.Hide();

            // Get scores
            int blueScore = Match.Instance.BlueTeamScore;
            int redScore = Match.Instance.RedTeamScore;
            // Get local player team
            Team localTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);


            int result = 0; // Draw
            Team winnerTeam;
            // Set winner text
            if (blueScore == redScore)
            {
                winnerString = "You Draw";
            }
            else
            {
                winnerTeam = (blueScore > redScore) ? Team.Blue : Team.Red;
                    
                if(winnerTeam == localTeam)
                {
                    result = 1; // You win
                    winnerString = "<color=#00C8FF>You</color> Win";
                }
                else
                {
                    result = -1; // You lose
                    winnerString = "<color=#00C8FF>You</color> Lose";
                }
            }

           

            // Fill text fields
            winnerText.text = winnerString;
            blueScoreText.text = blueScore.ToString();
            redScoreText.text = redScore.ToString();
            if (!PhotonNetwork.OfflineMode)
            {
                int points = 0;
                
                switch (result)
                {
                    case 0:
                        points = StatsManager.DrawPoints;
                        break;
                    case -1:
                        points = StatsManager.DefeatPoints;
                        break;
                    case 1:
                        points = StatsManager.VictoryPoints;
                        break;
                }
                pointsMessageText.text = string.Format(pointsMessageOnlineFormat, points);
            }
            else
            {
                pointsMessageText.text = pointsMessageOfflineFormat;
            }
                

        }

       
    }

}
