using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    public class ShootChoice : Choice
    {
        GameObject ball;
        float ballRadius;
        TeamHelper teamHelper;

        Vector3 target;
        bool hasTarget = false;
        DateTime lastShootTime;


        public ShootChoice(PlayerAI owner) : base(owner) 
        {
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);
            ballRadius = ball.GetComponent<SphereCollider>().radius;
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == Owner.Team);
        }

        public override void Evaluate()
        {
            
            Vector3 playerToBallV = ball.transform.position - Owner.FireWeapon.transform.position;
            if (playerToBallV.magnitude < Owner.FireWeapon.FireRange)
            {
                Weight = 2f;
            }
            else
            {
                Reset();
                Weight = 0;
            }
                
        }

      
        public override void StopPerformingAction()
        {
            Reset();
        }


        public override void PerformAction()
        {
            if (Match.Instance && Match.Instance.State != (int)MatchState.Started)
                return;

            //AI must choose where to shoot the ball
            if (!hasTarget)
            {
                Vector3 playerToBall = ball.transform.position - Owner.transform.position;

                bool defensivePosition = Vector3.Dot(playerToBall, teamHelper.transform.forward) < 0;
                bool onTheRight = Vector3.Dot(playerToBall, Vector3.right) < 0;

                hasTarget = true;
                target = ball.transform.position;
            }

            Owner.Sprint(false);

            // Check where is the ball in the field
            Vector3 ballToOppGoalLine = teamHelper.OpponentGoalLine.position - ball.transform.position;
            Vector3 ballToOwnedGoalLine = teamHelper.OwnedGoalLine.position - ball.transform.position;
            bool attacking = ballToOppGoalLine.sqrMagnitude - ballToOwnedGoalLine.sqrMagnitude > 0 ? true : false;

            // Adjust aim
            target = GetTargetToAim(attacking);

            //Vector3 dir = ball.transform.position - Owner.transform.position;
            //Vector3 targetFwd = dir;
            //targetFwd.y = 0;

            //Owner.transform.forward = Vector3.MoveTowards(Owner.transform.forward, targetFwd.normalized, Time.deltaTime * 10);
            //Owner.LookAtTheBall();
            Owner.LookAt(target);

            // Check aim
            //if(Owner.transform.forward == targetFwd.normalized)
            //{
                if( (DateTime.UtcNow - lastShootTime).TotalSeconds > 1f/Owner.FireWeapon.FireRate)
                {
                    Debug.Log("Try shoot....................");
                    lastShootTime = DateTime.UtcNow;
                    Owner.TryShoot();
                    
                    Reset();
                    
                }
                
            //}
            
        }

        void Reset()
        {
            
            hasTarget = false;
        }

        Vector3 GetTargetToAim(bool attacking)
        {
            Vector3 target = ball.transform.position;

            // Check if the ai is behind or in front of the ball
            Vector3 aiToBall = ball.transform.position - Owner.transform.position;
            if(Vector3.Dot(aiToBall, teamHelper.transform.forward) < 0)
            {
                // Is in front of the ball
                // We don't have a clear direction towards the goal line
                Debug.Log("In front of the ball");
            }
            else
            {
                // Is behind the ball
                // Check if there is some way to shoot in goal
                Vector3 goalToBall = ball.transform.position - teamHelper.OpponentGoalLine.position;
                Vector3 ballToAI = -aiToBall;
                if(Vector3.Angle(goalToBall.normalized, ballToAI.normalized) < 60)
                {
                    // We have a clear direction towards the goal line
                    Debug.Log("Behind the ball, aiming goal line");
                    // Find the direction to shoot the ball in goal
                    Vector3 dirV = goalToBall + goalToBall.normalized * 4 * ballRadius;
                    Vector3 origin = teamHelper.OpponentGoalLine.position + dirV;


                    Ray ray = new Ray(origin, -goalToBall.normalized);
                    Debug.DrawRay(ray.origin, ray.direction*10, Color.yellow, 5);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 10, LayerMask.GetMask(new string[] { Layer.Ball })))
                    {
                        // The point we found is on the other side of the ball
                        //Debug.DrawRay(Owner.transform.position, hit.point * aiToBall.magnitude, Color.blue, 5);

                        target = hit.point;
                    }
                }
                else
                {
                    // We don't have a clear direction towards the goal line
                    Debug.Log("Behind the ball");
                }
            }

            return target;
        }
    }

}
