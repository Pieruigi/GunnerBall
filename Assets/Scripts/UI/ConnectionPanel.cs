using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class ConnectionPanel : MonoBehaviourPunCallbacks
    {
        public static ConnectionPanel Instance { get; private set; }

        [SerializeField]
        GameObject panel;

        [SerializeField]
        Transform image;

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
                panel.SetActive(false);
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
            if (panel.activeSelf)
            {
                image.Rotate(new Vector3(0, 0, 100 * Time.deltaTime));
            }
        }

        public void Show(bool value)
        {
            panel.SetActive(value);
        }
    }

}
