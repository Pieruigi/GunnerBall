using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Zoca
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1.0";

        #region join_parameters
        int expectedMaxPlayers = 2;
        string roomName = null;
        #endregion

        private void Awake()
        {
            // Allow the master client to sync scene to other clients
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Connect();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Connect player and try to launch a quick game
        /// </summary>
        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                // Already connected to photon network, join a random room
                Debug.LogFormat("PUN - Joining or creating room...");
                //expectedMaxPlayers = 2;
                PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
            }
            else
            {
                // Connect to the photon network first
                Debug.LogFormat("PUN - Connecting to Photon network...");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        #region private

        #endregion

        #region callbacks
        public override void OnConnectedToMaster()
        {
            Debug.LogFormat("PUN - Connected to MasterServer.");
            // Joining or creating room
            Debug.LogFormat("PUN - Joining or creating room...");
            PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN - Disconnected from Photon network.");
        }

        public override void OnCreatedRoom()
        {
            Debug.LogFormat("PUN - New room created [Name:{0}].", PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("PUN - Unable to create room: [{0}] {1}.", returnCode, message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("PUN - Unable to join room: [{0}] {1}.", returnCode, message);
            
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("PUN - Unable to join random room: [{0}] {1}.", returnCode, message);
            if(returnCode == ErrorCode.NoRandomMatchFound)
            {
                // Create a new room
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = (byte)expectedMaxPlayers });
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.LogFormat("PUN - Room joined [Name:{0}].", PhotonNetwork.CurrentRoom.Name);
        }
        #endregion

    }

}
