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

        bool connecting = false;

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
            connecting = true;
           
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
            if (connecting)
            {
                // This callback is called even when you leave a game ( moving from game to 
                // master server ); in that case we don't want to join again, so we check
                // the connecting flag.
                connecting = false;
                Debug.LogFormat("PUN - Connected to MasterServer.");
                // Joining or creating room
                Debug.LogFormat("PUN - Joining or creating room...");
                PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
            }
            
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
            
            if(returnCode == ErrorCode.NoRandomMatchFound)
            {
                // Create a new room
                Debug.LogWarningFormat("PUN - Unable to join random room: [{0}] {1}.", returnCode, message);
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = (byte)expectedMaxPlayers });
            }
            else
            {
                Debug.LogErrorFormat("PUN - Unable to join random room: [{0}] {1}.", returnCode, message);
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.LogFormat("PUN - Room joined [Name:{0}].", PhotonNetwork.CurrentRoom.Name);

            // Set the team
            // The following code only support 1vs1
            Debug.LogFormat("PUN - local player actor number: {0}", PhotonNetwork.LocalPlayer.ActorNumber);
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomProperty.TeamColor, Team.Blue);
            }
            else
            {
                PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomProperty.TeamColor, Team.Red);
            }

            // Set the default character
            Debug.LogFormat("PUN - Setting default character id.");
            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomProperty.CharacterId, 0);
    
        }
        #endregion

    }

}
