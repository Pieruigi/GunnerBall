using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class PhotonConnection : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameObject panel;

        private void Awake()
        {
            panel.SetActive(true);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnConnectedToMaster()
        {
            panel.SetActive(false);
        }
    }

}
