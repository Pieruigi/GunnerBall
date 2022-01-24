using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class MainPanel : MonoBehaviourPunCallbacks
    {
        


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();

            if(PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
    }

}
