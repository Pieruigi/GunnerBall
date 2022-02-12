using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class PowerUpTemplate : MonoBehaviour
    {
        #region properties
        public Skill Skill
        {
            get { return skill; }
        }
        #endregion

        #region private fields
        Skill skill;
        float time;
        float remainingTime;
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
    
        }

        // Update is called once per frame
        void Update()
        {

        }

        //void PlayFxIn()
        //{
        //    DOTween
        //}

        #endregion

        #region public methods
        public void Init(Skill skill, float time, float remainingTime)
        {
            this.skill = skill;
            this.time = time;
            this.remainingTime = remainingTime;

            // Play fx
            //PlayFxIn();
        }
                

        #endregion
    }

}
