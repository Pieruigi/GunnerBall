using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    /// <summary>
    /// We simply switch shadows off and then we activate them in realtime ( usefull for baking )
    /// </summary>
    public class ShadowActivator : MonoBehaviour
    {
        

        private void Awake()
        {
            GetComponent<Light>().shadows = LightShadows.Soft;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
