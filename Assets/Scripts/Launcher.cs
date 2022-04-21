using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Zoca
{
    public class Launcher : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        public static Launcher Instance { get; private set; }

        string gameVersion = "1.0";

        #region join_parameters
        int expectedMaxPlayers = 2;
        string roomName = null;
        float matchLength = 300;

        bool updateRoomList = false;
            #endregion

        bool connecting = false;
       
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                // Allow the master client to sync scene to other clients
                PhotonNetwork.AutomaticallySyncScene = true;
                gameVersion = Application.version;

                // Join default lobby on awake.
                // We must wait for the client to join the default lobby in order to create or join
                // any room.
                //JoinDefaultLobby();
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

        private void LateUpdate()
        {
          
        }

        public void JoinDefaultLobby()
        {
           
            ConnectAndJoinDefaultLobby();
           
        }

        /// <summary>
        /// Creates a room 
        /// </summary>
        /// <param name="expectedMaxPlayers"></param>
        public void CreateRoom(int expectedMaxPlayers, int mapId)
        {
            

            PhotonNetwork.OfflineMode = false;
            this.expectedMaxPlayers = expectedMaxPlayers;


            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)expectedMaxPlayers };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomCustomPropertyKey.PlayerCreator, RoomCustomPropertyKey.MapId };
            roomOptions.IsVisible = true;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MatchLength, (int)matchLength);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.PlayerCreator, (string) PhotonNetwork.NickName);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MapId, (byte)mapId);

#if AI_SUPPORT
            // AI support
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.AICount, (byte)0);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.AINextCodeValue, (byte)0);
#endif
            PhotonNetwork.CreateRoom(null, roomOptions);

        }

        public void CreateRoomOffline(int maxPlayers, int mapId)
        {

            PhotonNetwork.OfflineMode = true;
            this.expectedMaxPlayers = maxPlayers;

            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)maxPlayers };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomCustomPropertyKey.MatchLength, RoomCustomPropertyKey.MapId };

            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MatchLength, (int)matchLength);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.PlayerCreator, (string)PhotonNetwork.NickName);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.MapId, (byte)mapId);
#if AI_SUPPORT
            // AI support
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.AICount, (byte)0);
            roomOptions.CustomRoomProperties.Add(RoomCustomPropertyKey.AINextCodeValue, (byte)0);
#endif
            PhotonNetwork.CreateRoom(null, roomOptions);

        }

        /// <summary>
        /// Joins a given room
        /// </summary>
        /// <param name="roomName"></param>
        public void JoinRoom(string roomName)
        {
            PhotonNetwork.OfflineMode = false;

            PhotonNetwork.JoinRoom(roomName);
        }
       
        /// <summary>
        /// Connect player and try to launch a quick game
        /// </summary>
        void ConnectAndJoinDefaultLobby()
        {
            connecting = true;
           
            if (PhotonNetwork.IsConnected)
            {
                // Already connected to photon network, join a random room
                Debug.LogFormat("PUN - Joining default lobby...");
                
                PhotonNetwork.JoinLobby();
                connecting = false;
            }
            else
            {
                // Connect to the photon network first
                Debug.LogFormat("PUN - Connecting to Photon network...");
                PhotonNetwork.GameVersion = gameVersion;
                Debug.LogFormat("PUN - GameVersion: " + PhotonNetwork.GameVersion);
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
                Debug.LogFormat("PUN - Joining lobby...");
                               
                PhotonNetwork.JoinLobby();
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
            Debug.LogErrorFormat("PUN - Unable to join random room: [{0}] {1}.", returnCode, message);
           
        }

        public override void OnJoinedLobby()
        {
          
            Debug.Log("Joined default lobby");

            
        }

        

        /// <summary>
        /// Called on the local player when entering the room
        /// </summary>
        public override void OnJoinedRoom()
        {
          
        }

        
        public override void OnLeftRoom()
        {
            // When you leave training room
            if (PhotonNetwork.OfflineMode || !PhotonNetwork.InLobby)
            {
                PhotonNetwork.OfflineMode = false;
                ConnectAndJoinDefaultLobby();
            }    
        }

#endregion


    }

}

