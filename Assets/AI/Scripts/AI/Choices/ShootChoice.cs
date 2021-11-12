using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    [System.Serializable]
    public class ShootChoice : Choice
    {
        GameObject ball;
        float ballRadius;
        TeamHelper teamHelper;

        Vector3 target;
        bool hasTarget = false;
        DateTime lastShootTime;
        Rigidbody ballRB;

        public ShootChoice(PlayerAI owner) : base(owner) 
        {
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);
            ballRB = ball.GetComponent<Rigidbody>();
            ballRadius = ball.GetComponent<SphereCollider>().radius * ball.transform.localScale.x;
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == Owner.Team);
        }

        public override void Evaluate()
        {
            
            Vector3 playerToBallV = ballRB.position - Owner.AimOrigin.position;
            if (playerToBallV.magnitude < Owner.AimRange * 0.8f)
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
                Vector3 playerToBall = ballRB.position - Owner.transform.position;

                bool defensivePosition = Vector3.Dot(playerToBall, teamHelper.transform.forward) < 0;
                bool onTheRight = Vector3.Dot(playerToBall, Vector3.right) < 0;

                hasTarget = true;
                target = ballRB.position;
            }

            Owner.Sprint(false);

            // Check where is the ball in the field
            Vector3 ballToOppGoalLine = teamHelper.OpponentGoalLine.position - ballRB.position;
            Vector3 ballToOwnedGoalLine = teamHelper.OwnedGoalLine.position - ballRB.position;
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
                if( (DateTime.UtcNow - lastShootTime).TotalSeconds > 1f/Owner.FireRate)
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
            Vector3 ballPos = ballRB.position;
            Vector3 target = ballPos;
            // The shoot it will take place in 100 millis, so we need to adjust the position
            //target += ballRB.velocity * 0.1f;


            // Check if the ai is behind or in front of the ball
            Vector3 aiToBall = ballPos - Owner.AimOrigin.position;
            if(Vector3.Dot(aiToBall, teamHelper.transform.forward) < 0)
            {
                // Is in front of the ball
                // We don't have a clear direction towards the goal line
                Debug.Log("In front of the ball");
                // Starting from the center of the ball we can aim to the right or to the left depending on the player
                // position; at most we can hit the tangent so we can find the maximum angle this way: 
                // ballRadius = ballDist * cosB; A = 90 - B; A is the angle we need
                Vector3 dir = aiToBall;
                
                float angleB = Mathf.Acos(ballRadius / dir.magnitude) * Mathf.Rad2Deg;
                float angleA = 90 - angleB;
                Debug.Log("AngleA:" + angleA);
                // Gived the direction between the ai and the all we can rotate between 0 and A ( or -A, depends
                // on the position )


                // Check whether the ai is to the right or to the left
                float teamSign = -Mathf.Sign(Vector3.Dot(aiToBall, Vector3.forward));
                Vector3 newDir;
                if (Vector3.Dot(aiToBall, Vector3.right) > 0)
                {
                    
                    // To the left
                    newDir = Quaternion.AngleAxis(teamSign * angleA/2, Vector3.up) * aiToBall;
                }
                else
                {
                    // To the right
                    newDir = Quaternion.AngleAxis(-angleA/2*teamSign, Vector3.up) * aiToBall;
                }

                // Apply rotation to the direction
                Debug.DrawRay(Owner.AimOrigin.position, newDir * 10, Color.black, 5);
                target = Owner.AimOrigin.position + newDir;

                // We want the ai to hit the ball in the bottom
                // We move the target point down and then we get the new target by connecting the stretched target to
                // the center of the ball
                // Stretch down the target point
                target += Vector3.down * UnityEngine.Random.Range(1f, 6f) * ballRadius;
                // Connect the new point to the center of the ball
                target = target - ball.transform.position;
                // Get the intersection
                target = ball.transform.position + target.normalized * ballRadius;
                Debug.DrawRay(Owner.AimOrigin.position, (target-Owner.AimOrigin.position).normalized * 10, Color.grey, 5);
                //Time.timeScale = 0;
            }
            else
            {
               
                // Is behind the ball
                // Check if there is some way to shoot in goal
                Vector3 goalToBall = ballPos - teamHelper.OpponentGoalLine.position;
                Vector3 ballToAI = -aiToBall;
                if(Vector3.Angle(goalToBall.normalized, ballToAI.normalized) < 80)
                {
                    Debug.Log("Behind the ball having shoot direction");
                    // We have a clear direction towards the goal line
                    //Debug.Log("Behind the ball, aiming goal line");
                    // Find the direction to shoot the ball in goal
                    Vector3 dirV = goalToBall + goalToBall.normalized * 4 * ballRadius;
                    Vector3 origin = teamHelper.OpponentGoalLine.position + dirV;


                    Ray ray = new Ray(origin, -goalToBall.normalized);
                    Debug.DrawRay(ray.origin, ray.direction*100, Color.yellow, 5);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 10, LayerMask.GetMask(new string[] { Layer.Ball })))
                    {
                        
                        target = hit.point;
                        
                    }
                }
                else
                {
                    // We don't have a clear direction towards the goal line
                    Debug.Log("Behind the ball");
                }
            }
            //target = ballRB.position;
            return target;
        }
    }

}
