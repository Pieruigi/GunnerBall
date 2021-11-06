using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    public enum ChoiceWeight { None, Low, Medium, High, VeryHigh }

    /// <summary>
    /// Choices can be made by more than one action; for example, the choice "shoot in goal" can be made by 
    /// "move to a convenient spot" and "shoot".
    /// List of actions:
    ///     Idle
    ///     ShootPass
    ///     ShootGoal
    ///     ShootThrowAway
    ///     ShootFreeze
    ///     
    /// 
    /// </summary>
    public abstract class Choice
    {
        #region properties
        public float Weight
        {
            get { return weight; }
            protected set { weight = value + Random.Range(-weightErr, weightErr); }
        }

        public PlayerAI Owner
        {
            get { return owner; }
        }
        #endregion

        #region private fields
        float weight = 0;
        float weightErr = .3f;
        PlayerAI owner;
        #endregion

        #region abstract methods
        // Call every time you want to make a decision; this method should set the appropriate weight value
        public abstract void Evaluate();

        // Call every time you want to perform this action
        public abstract void PerformAction();

       
        public abstract void StopPerformingAction();

        #endregion

        public Choice(PlayerAI owner)
        {
            this.owner = owner;
        }

        public override string ToString()
        {
            return string.Format("[{0} Weight:{1}]", this.GetType().Name, weight);
        }

    }

   

}
