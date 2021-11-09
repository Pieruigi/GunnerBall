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
        
        
        float sprintElapsed = 0;
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
            {
                Weight = 1;
            }
            else
            {
                Weight = 0;
                Reset();
            }
                
            
        }

        //public override void StartPerformingAction()
        //{
        //}

        public override void StopPerformingAction()
        {
            Reset();
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
                Vector3 aiToPosV = targetPosition - Owner.FireWeapon.transform.position;// Vector from the ai to the pos 
                float angle = Vector3.Angle(Owner.transform.forward, targetPosition);
                if (aiToPosV.magnitude > Owner.FireWeapon.FireRange && angle < 40)
                    sprinting = true;

                // 2. sprint to denfend
                // At the moment the same ( we could just spare a little of stamina in case )
            }
           
          
            if (sprinting)
            {
                if (!Owner.Sprinting)
                {
                    // It may happen that when the AI start sprinting the target destination has not been updated due to
                    // the reaction time; in this case the AI can turn back because it starts looking at the destination 
                    // rather than the ball.
                    if (sprintElapsed < Owner.ReactionTime)
                    {
                        sprintElapsed += Time.deltaTime;
                    }
                    else
                    {
                        Owner.Sprint(sprinting);
                    }
                }
                    
            }
            else
            {
                sprintElapsed = 0;
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
            
            waypoint = teamHelper.GetTheClosestFormationHelper().GetChild(Owner.WaypointIndex);
            
        }

        void SetTargetPosition()
        {
            //if (Ball.Instance)
            //    ball = Ball.Instance.gameObject;
            //if (ball == null)
            //    ball = GameObject.FindGameObjectWithTag(Tag.Ball);

            Vector3 oldTargetPosition = targetPosition;
            
            // Position from the first helper
            targetPosition = new Vector3(waypoint.position.x - waypoint.parent.position.x, 0, waypoint.position.z - waypoint.parent.position.z);
            targetPosition += ball.transform.position;// + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            targetPosition.y = 0;

            if (Vector3.SqrMagnitude(targetPosition - oldTargetPosition) < 1)
                targetPosition = oldTargetPosition;
        }

        void Reset()
        {
            Owner.Sprint(false);
            sprintElapsed = 0;

        }
        
    }

}
