using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{


    public class PowerUpManager : MonoBehaviour
    {
        #region internal classes
        class Data
        {
            public float buff;
            public float time;
        }
        #endregion

        #region properties

        #endregion

        #region private
        const int PowerUpMax = 3;

        List<Data> datas = new List<Data>(PowerUpMax);
        #endregion

        #region private methods
        private void Awake()
        {
            // Clear all
            Reset();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Reset()
        {
            datas.Clear();
        }
        #endregion

        #region public methods
        public void PowerUp(PowerUp powerUp)
        {
            Debug.Log("Activating power up");
            
        }

        public bool CanBePoweredUp()
        {
            return datas.Count < PowerUpMax ? true : false;
        }
        #endregion
    }

}
