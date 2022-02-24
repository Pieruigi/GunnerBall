using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class OnGoalDestroyer : MonoBehaviour
    {
        [SerializeField]
        float delay = 0.5f;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void OnEnable()
        {
            Match.Instance.OnStateChanged += HandleOnGoal;
        }

        private void OnDisable()
        {
            Match.Instance.OnStateChanged -= HandleOnGoal;
        }

        // Update is called once per frame
        void Update()
        {

        }


        void HandleOnGoal()
        {
            if (Match.Instance.State == (int)MatchState.Paused) Destroy(gameObject, delay); 
        }
    }

}
