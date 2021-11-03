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
        float matchLength = 300;
        #endregion

        bool connecting = false;

        private void Awake()
        {
            // Allow the master client to sync scene to other clients
            PhotonNetwork.AutomaticallySyncScene = true;
            gameVersion = Application.version;
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

        public void QuickMatch(int expectedMaxPlayers)
        {
            PhotonNetwork.OfflineMode = false;
            this.expectedMaxPlayers = expectedMaxPlayers;

            Connect();
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
                Join();
                //PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
            }
            else
            {
                // Connect to the photon network first
                Debug.LogFormat("PUN - Connecting to Photon network...");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void Join()
        {
            // We must read the choosen join mode first
            PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
        }

        public void LaunchOffline(int maxPlayers)
        {
            
            PhotonNetwork.OfflineMode = true;
            
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)maxPlayers };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomCustomPropertyKey.MatchLength };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MatchLength, (int)matchLength);

            PhotonNetwork.CreateRoom(null, roomOptions);

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
                //PhotonNetwork.JoinRandomRoom(null, (byte)expectedMaxPlayers);
                Join();
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
                // Prepare room custom property hashtable
                RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)expectedMaxPlayers };
                roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomCustomPropertyKey.MatchLength };
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MatchLength, (int)matchLength);

                PhotonNetwork.CreateRoom(null, roomOptions);
            }
            else
            {
                Debug.LogErrorFormat("PUN - Unable to join random room: [{0}] {1}.", returnCode, message);
            }
        }

        /// <summary>
        /// Called on the local player when entering the room
        /// </summary>
        public override void OnJoinedRoom()
        {
            //Debug.LogFormat("PUN - Room joined [Name:{0}].", PhotonNetwork.CurrentRoom.Name);

            // Set the team
            // We fill the blue team with the first players, and the red team with the last ones.
            //Debug.LogFormat("PUN - local player actor number: {0}", PhotonNetwork.LocalPlayer.ActorNumber);
            ////if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            //if (PhotonNetwork.LocalPlayer.ActorNumber <= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            //{
            //    //PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Blue);
            //    PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor, Team.Blue);
            //}
            //else
            //{
            //    //PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Red);
            //    PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor, Team.Red);
            //}

            //// Set the default character
            //Debug.LogFormat("PUN - Setting default character id.");
            //PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, 0);
            //PlayerCustomPropertyUtility.SynchronizePlayerCustomProperties(PhotonNetwork.LocalPlayer);
        }
        #endregion

    }

}
