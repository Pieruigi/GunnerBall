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

        bool checkDisabled = false;

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
            // Already checked
            if (checkDisabled)
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
                    //// The other team sets a score
                    //if (team == Team.Blue)
                    //    Match.Instance.TeamScored(Team.Red);
                    //else
                    //    Match.Instance.TeamScored(Team.Blue);

                    Match.Instance.Goal(team == Team.Blue ? Team.Red : Team.Blue);
                }
            }


        }
    }

}
