using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField]
        Team team = Team.Blue;

   
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (Match.Instance.State != (int)MatchState.Started)
                return;

            // Check for the ball
            if (!Tag.Ball.Equals(other.tag))
                return;

            // Only the master client checks for goal
            if (PhotonNetwork.IsMasterClient)
            {
                // The ball must enter from the front side
                Vector3 triggerToBallDirection = other.gameObject.transform.position - transform.position;

                // Dot product
                if(Vector3.Dot(transform.forward, triggerToBallDirection) > 0)
                {
                    

                    Match.Instance.Goal(team == Team.Blue ? Team.Red : Team.Blue);
                }
            }

            Ball ball = other.GetComponent<Ball>();
            if (ball.LastHitter != null)
            {
                PlayerController hitter = ball.LastHitter.GetComponent<PlayerController>();
                if(hitter.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    if(team != (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor))
                    {
                        //PlayerController.Local.FireWeapon.IncreaseSuperShotCharge();
                        //PlayerController.Local.FireWeapon.IncreaseSuperShotCharge();
                    }
                        
                }
            }

        }
    }

}
