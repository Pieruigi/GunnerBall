using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    public class ShootChoice : Choice
    {
        GameObject ball;

        public ShootChoice(PlayerAI owner) : base(owner) 
        {
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);
        }

        public override void Evaluate()
        {
            
            Vector3 playerToBallV = ball.transform.position - Owner.transform.position;
            if (playerToBallV.magnitude < Owner.PlayerController.FireWeapon.FireRange)
                Weight = 2f;
            else
                Weight = 0;
        }

        public override void PerformAction()
        {
            Debug.Log("Shoot");
            Owner.PlayerController.Sprint(false);
            Owner.PlayerController.LookAtTheBall();
        }
    }

}
