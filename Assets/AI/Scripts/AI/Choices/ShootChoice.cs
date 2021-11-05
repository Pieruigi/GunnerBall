using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    public class ShootChoice : Choice
    {
        GameObject ball;
        TeamHelper teamHelper;

        Vector3 target;
        bool hasTarget = false;
        float shootCooldown = 0;

        public ShootChoice(PlayerAI owner) : base(owner) 
        {
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == Owner.Team);
        }

        public override void Evaluate()
        {
            
            Vector3 playerToBallV = ball.transform.position - Owner.transform.position;
            if (playerToBallV.magnitude < Owner.FireWeapon.FireRange)
            {
                Weight = 2f;
            }
            else
            {
                hasTarget = false;
                Weight = 0;
            }
                
        }

        public override void PerformAction()
        {
            if(shootCooldown > 0)
            {
                shootCooldown -= Time.deltaTime;
            }

            // AI must choose where to shoot the ball
            if (!hasTarget)
            {
                Vector3 playerToBall = ball.transform.position - Owner.transform.position;

                bool defensivePosition = Vector3.Dot(playerToBall, teamHelper.transform.forward) < 0;
                bool onTheRight = Vector3.Dot(playerToBall, Vector3.right) < 0;

                hasTarget = true;
                target = ball.transform.position;
            }

            Owner.Sprint(false);



            // Adjust aim
            //forwardTarget = defensive;
            //if(defensivePosition)
            //{
            //    // Player is between the ball and the opponent goal line
            //    if (onTheRight)
            //    {

            //    }
            //}
            //else
            //{
            //    // The ball is between the player and the opponent goal line

            //}

            Vector3 dir = ball.transform.position - Owner.transform.position;
            Vector3 targetFwd = dir;
            targetFwd.y = 0;
            
            Owner.transform.forward = Vector3.MoveTowards(Owner.transform.forward, targetFwd.normalized, Time.deltaTime * 10);

            // Check aim
            if(Owner.transform.forward == targetFwd.normalized)
            {
                if(shootCooldown <= 0)
                {
                    shootCooldown = 1;
                    Debug.DrawRay(Owner.transform.position, Owner.transform.forward * 10, Color.red, 5);
                    hasTarget = false;
                }
                
            }
            //else
            //{
            //    Owner.PlayerController.LookAtTheBall();
            //}

           


        }
    }

}
