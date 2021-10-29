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

        #endregion

        public IdleChoice(PlayerAI owner) :base(owner) 
        {
            teamHelper = new List<TeamHelper>(GameObject.FindObjectsOfType<TeamHelper>()).Find(t => t.Team == owner.Team);
            ball = GameObject.FindGameObjectWithTag(Tag.Ball);
        }  

        public override void Evaluate()
        {
            Weight = 1;
        }

        public override void PerformAction()
        {
            // We interpolate the player position with the two closest helpers
            // Waypoint from the two closest helpers
            Transform waypoint = teamHelper.FormationHelpers[0].GetChild(Owner.WaypointIndex);
            Transform waypoint2 = teamHelper.FormationHelpers[1].GetChild(Owner.WaypointIndex);

            // Position from the first helper
            Vector3 pos = new Vector3(waypoint.position.x - teamHelper.FormationHelpers[0].position.x, 0,  waypoint.position.z - teamHelper.FormationHelpers[0].position.z);
            pos += ball.transform.position;
            pos.y = 0;

            // Position from the second helper
            Vector3 pos2 = new Vector3(waypoint2.position.x - teamHelper.FormationHelpers[1].position.x, 0, waypoint2.position.z - teamHelper.FormationHelpers[1].position.z);
            pos2 += ball.transform.position;
            pos2.y = 0;

            // Closest->Ball projected on closets->closets2
            Vector3 closestToBall = ball.transform.position - teamHelper.FormationHelpers[0].position;
            Vector3 closestToClosest2 = teamHelper.FormationHelpers[1].position - teamHelper.FormationHelpers[0].position;
            float ballDistProj = Vector3.Dot(closestToBall, closestToClosest2);
            // Position interpolation
            pos = Vector3.Lerp(pos, pos2, closestToBall.magnitude / closestToClosest2.magnitude);

            // Move to destination
            Owner.PlayerController.MoveTo(pos);

        }
    }

}
