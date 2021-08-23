using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class ConnectionMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        Button buttonExit;

        [SerializeField]
        Text textMessage;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

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
            textMessage.text = "Connecting...";
        }
        
        public void Exit()
        {
            // Behaviour depends on the connection state
            PhotonNetwork.Disconnect();

        }

        #region photon_callbacks
        public override void OnJoinedRoom()
        {
            textMessage.text = "Waiting for opponent...";
        }
        #endregion
    }

}
