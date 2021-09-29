using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class TeamScore : MonoBehaviour
    {
        [SerializeField]
        Team team;

        [SerializeField]
        Text textScore;

        //string textFormat = "{0} : {1}";
        string textFormat = "{0}";
        bool isYou = false;

        // Start is called before the first frame update
        void Start()
        {
            //if (team == Team.Blue)
            //    textScore.color = Color.blue;
            //else
            //    textScore.color = Color.red;

            if ((team == Team.Blue && PhotonNetwork.LocalPlayer.ActorNumber == 1) ||
               (team == Team.Red && PhotonNetwork.LocalPlayer.ActorNumber == 0))
            {
                isYou = true; 
            }
            else
            {
                isYou = false;
            }

        }

        // Update is called once per frame
        void Update()
        {
            //textScore.text = string.Format(textFormat, isYou ? "You" : "Opp", team == Team.Blue ? Match.Instance.BlueTeamScore : Match.Instance.RedTeamScore);
            textScore.text = string.Format(textFormat, team == Team.Blue ? Match.Instance.BlueTeamScore : Match.Instance.RedTeamScore);
        }
    }

}
