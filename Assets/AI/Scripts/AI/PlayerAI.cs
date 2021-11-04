//#define TEST
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.AI
{
    public enum PlayerBehaviour { Neutral, Defensive, Offensive }

    public class PlayerAI : MonoBehaviour
    {
        #region inspector
        [SerializeField]
        float reactionTime = 0.25f;

        [SerializeField]
        Team team = Team.Blue;
        
        [SerializeField]
        PlayerBehaviour behaviour = PlayerBehaviour.Neutral;
        
        #endregion

        #region properties
        public Team Team
        {
            get { return team; }
            set { team = value; }
        }
        public PlayerBehaviour Behaviour
        {
            get { return behaviour; }
        }
        public int WaypointIndex
        {
            get { return waypointIndex; }
        }
        public bool Activated
        {
            get { return !deactivated; }
        }
#if TEST
        public FakePlayerController PlayerController
        {
            get { return playerController; }
        }
#else
        public PlayerController PlayerController
        {
            get { return playerController; }
        }
#endif
        #endregion

        #region private fields
        bool deactivated = true;
        List<Choice> choices; // The list of all the available choices
        Choice lastChoice;

        DateTime lastReaction;

        int waypointIndex = 0;

#if TEST
        FakePlayerController playerController;
#else
        PlayerController playerController;
#endif
        #endregion

        #region private
        private void Awake()
        {
            // Get player controller
#if TEST
            playerController = GetComponent<FakePlayerController>();
#else
            playerController = GetComponent<PlayerController>();
#endif

            choices = new List<Choice>();
            choices.Add(new IdleChoice(this));
        }
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (deactivated)
                return;

            if ((DateTime.UtcNow - lastReaction).TotalSeconds > reactionTime)
            {
                EvaluateChoices(); // Update choices weights

                lastChoice = GetBestChoice(); // Get the more convenient choice

                lastChoice?.PerformAction(); // Perform action


                //PrintLog(); /// Only for test
            }
        }

        void EvaluateChoices()
        {
            foreach (Choice choice in choices)
                choice.Evaluate();
        }

        Choice GetBestChoice()
        {
            float max = 0;
            Choice ret = null;
            foreach (Choice choice in choices)
            {
                if (max == 0)
                {
                    if (choice.Weight > 0)
                    {
                        ret = choice;
                        max = ret.Weight;
                    }
                }
                else
                {
                    if (choice.Weight > max)
                    {
                        ret = choice;
                        max = ret.Weight;
                    }
                }

            }

            return ret;

        }
        #endregion

        #region public
        public void SetWaypointIndex(int index)
        {
            waypointIndex = index;
        }

        public void Activate()
        {
            deactivated = false;
        }

        public void Deactivate()
        {
            deactivated = true;
        }

        #endregion

        #region debug
        void PrintLog()
        {
            foreach (Choice c in choices)
                Debug.Log(c);
        }
#endregion
    }

}
