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


        //[SerializeField]
        //PlayerBehaviour behaviour = PlayerBehaviour.Neutral;
        
        #endregion

        #region properties
      
        public float ReactionTime
        {
            get { return reactionTime; }
        }
        public Team Team
        {
            get { return team; }
            set { team = value; }
        }
        //public PlayerBehaviour Behaviour
        //{
        //    get { return behaviour; }
        //}
        public int WaypointIndex
        {
            get { return waypointIndex; }
        }
        public bool Activated
        {
            get { return !deactivated; }
        }
        public bool Sprinting
        {
            get { return playerController.Sprinting; }
        }
        //public FireWeapon FireWeapon
        //{
        //    get { return playerController.FireWeapon; }
        //}
        public float FireRate
        {
            get { return playerController.FireWeapon.FireRate; }
        }
        public Transform AimOrigin
        {
#if !TEST
            get { return playerController.PlayerCamera.transform; }
#else
            get { return playerController.FireWeapon.transform; }
#endif
        }
        public float AimRange
        {
#if !TEST
            get { return playerController.PlayerCamera.DistanceAdjustment + playerController.FireWeapon.FireRange; }
#else
            get { return playerController.FireWeapon.FireRange; }
#endif
        }
//#if TEST
//        public FakePlayerController PlayerController
//        {
//            get { return playerController; }
//        }
//#else
//        public PlayerController PlayerController
//        {
//            get { return playerController; }
//        }
//#endif
#endregion

#region private fields
        bool deactivated = true;
       
        List<Choice> choices; // The list of all the available choices
        
        Choice currentChoice;

        DateTime lastReaction;

        int waypointIndex = 0;

        // Destination 
        Vector3 destination;
        bool hasDestination = false;
        float minDistSqr = 4f;
     

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
            deactivated = false;
#else
            playerController = GetComponent<PlayerController>();
#endif

            choices = new List<Choice>();
            choices.Add(new IdleChoice(this));
            choices.Add(new ShootChoice(this));
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

            // Check if the AI has a destination to reach
            if (hasDestination)
            {
                Vector3 v = destination - transform.position;
               
                if (v.sqrMagnitude < minDistSqr)
                {
                    hasDestination = false;
                    playerController.Move(false, Vector2.zero);

                }
                else
                {
                    //moveDir = new Vector2(v.x, v.z);
                    playerController.Move(true, new Vector2(v.x, v.z).normalized);

                }
            }


            if ((DateTime.UtcNow - lastReaction).TotalSeconds > reactionTime)
            {
                EvaluateChoices(); // Update choices weights

                Choice lastChoice = currentChoice;
                currentChoice = GetTheBestChoice(); // Get the more convenient choice

                // If the current choice is not the last choice we must stop the last choice
                if(lastChoice != currentChoice)
                {
                    lastChoice?.StopPerformingAction();
                    //currentChoice?.StartPerformingAction();
                }

                //currentChoice?.PerformAction(); // Perform action

                Debug.Log("CurrentChoice:" + currentChoice);
                PrintLog(); /// Only for test
            }

            currentChoice?.PerformAction(); // Perform action
        }
        IEnumerator StopShooting()
        {
            yield return null;
            playerController.Shoot(false);
        }

        void EvaluateChoices()
        {
            foreach (Choice choice in choices)
                choice.Evaluate();
        }

        Choice GetTheBestChoice()
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

        public void ResetDestination()
        {
            hasDestination = false;
        }

        public void MoveTo(Vector3 destination)
        {
            // Get distance
            //Debug.Log("Move to " + destination);
            //float sqrDist = Vector3.SqrMagnitude(transform.position - destination);

            hasDestination = true;
            this.destination = destination;
         
            //playerController.MoveTo(destination);
        }

        public void Sprint(bool value)
        {
            playerController.Sprint(value);
        }

        public bool CanSprint()
        {
            return playerController.Stamina > 0;
        }

        public void LookAtTheBall()
        {
   

#if TEST
                playerController.LookAt(GameObject.FindGameObjectWithTag(Tag.Ball).transform.position);
#else
                playerController.LookAt(Ball.Instance.transform.position);
            
#endif
      

            //transform.forward = new Vector3(aiToBallV.x, 0, aiToBallV.z);


        }

        public void LookAt(Vector3 target)
        {
            playerController.LookAt(target);
        }

        public void TryShoot()
        {
            Debug.Log("AI - try shoot:" + gameObject.name);
#if TEST
           
                playerController.Shoot(true);
#else

                playerController.Shoot(true);
                StartCoroutine(StopShooting());    
#endif
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
