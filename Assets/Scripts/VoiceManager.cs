using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class VoiceManager : MonoBehaviour
    {
        public static VoiceManager Instance { get;  private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
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
