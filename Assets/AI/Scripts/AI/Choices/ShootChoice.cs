//#define TEST_AI
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    [System.Serializable]
    public class ShootChoice : Choice
    {
#if !TEST_AI
        Ball ball;
#else
        FakeBall ball;
#endif
        float ballRadius;
        TeamHelper teamHelper;

        Vector3 target;
     
        Rigidbody ballRB;
        float lastTargetDistance = -1;

        public ShootChoice(PlayerAI owner) : base(owner) 
        {
#if !TEST_AI
            ball = Ball.Instance;
#else
            ball = FakeBall.Instance;
#endif
            ballRB = ball.GetComponent<Rigidbody>();
            ballRadius = ball.GetComponent<SphereCollider>().radius * ball.transform.localScale.x;
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == Owner.Team);
        }

        public override void Evaluate()
        {
            
            Vector3 aiToBallV = ballRB.position - Owner.AimOrigin.position;

            // If the ball is too high only try to shoot when too close to the owned goal line
            float maxShootPitchInRadians = 45 * Mathf.Deg2Rad;
            // If B is the high of the ball and A the distance from the ball projected on the floor then B/A < tg(maxpitch)
            float bDivA = Mathf.Tan(maxShootPitchInRadians);
            
            float b = ball.transform.position.y;
            float a = new Vector2(aiToBallV.x, aiToBallV.z).magnitude;
            
            if (b/a > bDivA)
            {
                Reset();
                Weight = 0;
                return;
            }

            if (aiToBallV.magnitude < Owner.AimRange * 1.4f)
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

            // Ball is the target
            target = ballRB.position;

            
            Vector3 aiToBall = ballRB.position - Owner.AimOrigin.position;
            if (IsTooFarAway())
            {
                // Check the direction the ai is sprinting
                if(Vector3.Angle(Owner.transform.forward, new Vector3(aiToBall.x, 0, aiToBall.z).normalized) < 20)
                {
                    Owner.Sprint(true);
                    //Owner.Sprint(false);
                }
                else
                {
                    Owner.Sprint(false);
                }
                Owner.MoveTo(target);
            }
            else
            {
                Owner.Sprint(false);

                // Check where is the ball in the field
                Vector3 ballToOppGoalLine = teamHelper.OpponentGoalLine.position - ballRB.position;
                Vector3 ballToOwnedGoalLine = teamHelper.OwnedGoalLine.position - ballRB.position;
                bool attacking = ballToOppGoalLine.sqrMagnitude - ballToOwnedGoalLine.sqrMagnitude > 0 ? true : false;

                // Adjust aim
                target = GetTargetToAim(attacking);

                //AddError(ref target);

                // Aim the target
                Owner.LookAt(target);
                Owner.MoveTo(target);

                if (Owner.CanShoot() && (Owner.AimOrigin.position - ballRB.position).sqrMagnitude < Mathf.Pow(Owner.AimRange,2))
                {
                    Debug.Log("Try shoot....................");
                    
                    Owner.TryShoot();

                    Reset();

                }
            }
                

           

            
        }

        void AddError(ref Vector3 target)
        {
            float maxError = 0.35f;
            float factor = 0.1f; // The higher the more the error grows
            float ballSpeed = ballRB.velocity.magnitude;
            Vector3 error = new Vector3(UnityEngine.Random.Range(-maxError, maxError), UnityEngine.Random.Range(-maxError, maxError), UnityEngine.Random.Range(-maxError, maxError));

            error *= ballSpeed * factor;
            
            target += error;
        }

        bool IsTooFarAway()
        {
            Vector3 aiToBall = ballRB.position - Owner.AimOrigin.position;
            float sqrTargetDistance = aiToBall.sqrMagnitude;
            // Dot > 0 means the ball is going away
            float aiFwdBallVelDot = Vector3.Dot(ballRB.velocity, aiToBall);



            // Ball is going away
            if (aiFwdBallVelDot >= 0 && sqrTargetDistance > Mathf.Pow(Owner.AimRange * 0.9f, 2))
                return true;

            // Ball is coming towards the player
            if (aiFwdBallVelDot < 0 && sqrTargetDistance > Mathf.Pow(Owner.AimRange * 1.3f, 2))
                return true;

            return false;
        }

        void Reset()
        {
            lastTargetDistance = -1;
            
        }

        Vector3 GetTargetToAim(bool attacking)
        {
            Vector3 ballPos = ballRB.position;
            Vector3 target = ballPos;
           
            // Check if the ai is behind or in front of the ball
            Vector3 aiToBall = ballPos - Owner.AimOrigin.position;
            Vector3 goalToBall = ballPos - teamHelper.OpponentGoalLine.position;
            Vector3 ballToAI = -aiToBall;
            
            if (Vector3.Dot(aiToBall, teamHelper.transform.forward) < 0 || Vector3.Angle(goalToBall.normalized, ballToAI.normalized) > 70)
            {
                // In front of the ball
                // We can't shoot on goal from here, so we try to hit the ball from the bottom
                // Starting from the center of the ball we can aim to the right or to the left depending on the player
                // position; at most we can hit the tangent so that we can find the maximum angle this way: 
                // ballRadius = ballDist * cosB; A = 90 - B; A is the angle we need
                Vector3 dir = aiToBall;
                
                float angleB = Mathf.Acos(ballRadius / dir.magnitude) * Mathf.Rad2Deg;
                float angleA = 90 - angleB;
                // Gived the direction between the ai and the all we can rotate between 0 and A ( or -A, depends
                // on the position )

                // angleDir = 1 if ai is in front of the ball
                float angleDir = Vector3.Dot(aiToBall, teamHelper.transform.forward) < 0 ? 1 : -1;

                // Check whether the ai is to the right or to the left
                float teamSign = -Mathf.Sign(Vector3.Dot(aiToBall, Vector3.forward));
                Vector3 newDir;
                if (Vector3.Dot(aiToBall, Vector3.right) > 0)
                {
                    
                    // To the left
                    newDir = Quaternion.AngleAxis(teamSign * angleA * 0.85f * angleDir, Vector3.up) * aiToBall;
                }
                else
                {
                    // To the right
                    newDir = Quaternion.AngleAxis(-angleA * 0.85f * teamSign * angleDir, Vector3.up) * aiToBall;
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
                target = target - ballRB.position;
                // Get the intersection
                target = ballRB.position + target.normalized * ballRadius;

                Debug.DrawRay(Owner.AimOrigin.position, (target-Owner.AimOrigin.position).normalized * 30, Color.grey, 5);
                //Time.timeScale = 0;
            }
            else
            {
               
                    // We can shoot on goal from here
                    // Find the direction to shoot in goal
                    Vector3 dirV = goalToBall + goalToBall.normalized * (ballRadius + 0.5f);
                    Vector3 origin = teamHelper.OpponentGoalLine.position + dirV;

                    // Ray goes from ball to goal line
                    Ray ray = new Ray(origin, -goalToBall.normalized);
                    Debug.DrawRay(ray.origin, ray.direction*100, Color.yellow, 5);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 10, LayerMask.GetMask(new string[] { Layer.Ball })))
                    {
                        target = hit.point;
                        
                        // We need now to adjust aim by taking into account the actual ball velocity ( ex. if the ball
                        // is moving to the right it could keep moving to the right even after we shoot )
                        // We simulate to hit the ball to get the new velocity
                        Vector3 simVel = ball.ComputeNewVelocity(Owner.FirePower, target, hit.normal);
                        Debug.DrawRay(ray.origin, simVel, Color.black, 5);
                        // To avoid computing drag we simply lerp the simVel towards the targetVel
                        simVel = simVel.magnitude * Vector3.Lerp(simVel.normalized, -goalToBall.normalized, 0.5f);
                        Debug.DrawRay(ray.origin, simVel, Color.blue, 5);
                        // Get the velocity we must apply to the ball in order to reach the target velocity; 
                        Vector3 velDirToApply = -2*goalToBall.normalized - simVel.normalized; // Approximation
                        Debug.DrawRay(ray.origin, velDirToApply*100, Color.white, 5);
                        // Compute the target point
                        target = ballRB.position - velDirToApply * ballRadius;
                        
                    }

                    //Time.timeScale = 0f;
                    

               
            }

            AddError(ref target);
            //Time.timeScale = 0f;
            return target;
        }
    }

}
