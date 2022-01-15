using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class Version : MonoBehaviour
    {
       
        private void Awake()
        {
            string txt = "Alpha {0}";
            GetComponent<Text>().text = string.Format(txt, Application.version);
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
