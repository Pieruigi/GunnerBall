using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI 
{
    /// <summary>
    /// In idle the AI simply follow the ball trying to keep a convenient spot; for example a stricker
    /// would stay in front of the ball, while a defender would stay back.
    /// 
    /// </summary>
    public class IdleChoice : Choice
    {
        #region private fields
        TeamHelper teamHelper;
        GameObject ball;

        bool interpolateWaypoints = false;

        Transform waypoint;
        Vector3 targetPosition;

        #endregion

        public IdleChoice(PlayerAI owner) :base(owner) 
        {
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == owner.Team);
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);

            
            
        }  

        public override void Evaluate()
        {
            SetTargetWaypoint();
            SetTargetPosition();
            if ((Owner.transform.position - targetPosition).magnitude > Owner.FireWeapon.FireRange)
                Weight = 1;
            else
                Weight = 0;
            
        }

        public override void PerformAction()
        {
            if (!waypoint)
                return;

         

            // Need to sprint?
            // We sprint when:
            // 1. the ai is too far away from the ball
            // 2. the ball is running towards the goal line defended by the ai
            bool sprinting = false;
           
            //if (Owner.PlayerController.Stamina /*/ Owner.PlayerController.StaminaMax*/ > 0f)
            if(Owner.CanSprint())
            {
                // 1. too far away
                Vector3 aiToPosV = targetPosition - Owner.transform.position;// Vector from the ai to the pos 
                if (aiToPosV.magnitude > Owner.FireWeapon.FireRange)
                    sprinting = true;

                // 2. sprint to denfend
                // At the moment the same ( we could just spare a little of stamina in case )
            }
            
            if(sprinting)
            {
                if(!Owner.Sprinting)
                    Owner.Sprint(sprinting);
            }
            else
            {
                if (Owner.Sprinting)
                    Owner.Sprint(sprinting);
            }
                

            if (!Owner.Sprinting)
            {
                Owner.LookAtTheBall();
            }



            // Move to destination
            //Owner.PlayerController.MoveTo(targetPosition);
            Owner.MoveTo(targetPosition);



        }

        void SetTargetWaypoint()
        {
            Debug.Log("Closest:" + teamHelper.GetTheClosestFormationHelper());
            waypoint = teamHelper.GetTheClosestFormationHelper().GetChild(Owner.WaypointIndex);
        }

        void SetTargetPosition()
        {
            if (ball == null)
                ball = GameObject.FindGameObjectWithTag(Tag.Ball);

            // Position from the first helper
            targetPosition = new Vector3(waypoint.position.x - waypoint.parent.position.x, 0, waypoint.position.z - waypoint.parent.position.z);
            targetPosition += ball.transform.position;// + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            targetPosition.y = 0;

            
        }

    }

}
