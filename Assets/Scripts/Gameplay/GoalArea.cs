using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class GoalArea : MonoBehaviour
    {
        [SerializeField]
        Team team;
        public Team Team
        {
            get { return team; }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (Tag.Player.Equals(other.tag))
                other.GetComponent<PlayerController>().EnterGoalArea(this);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAA");
            if (Tag.Player.Equals(other.tag))
                other.GetComponent<PlayerController>().ExitGoalArea(this);
        }

        
    }

}
