using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class OfflinePanel: MonoBehaviourPunCallbacks
    {
        #region private fields
        [SerializeField]
        Button button1vs1;

        [SerializeField]
        Button button2vs2;

        [SerializeField]
        Transform roomListContent;

        [SerializeField]
        GameObject mapSelectorPanel;

        [SerializeField]
        GameObject lobbyPanel;

        GameObject roomListTemplate;
        DateTime lastRoomListUpdate;
        List<GameObject> rooms = new List<GameObject>();
        #endregion

        #region private methods
        private void Start()
        {
            //launcher = FindObjectOfType<Launcher>();

            // Get the room list element template and deactivate it
            roomListTemplate = roomListContent.GetChild(0).gameObject;
            roomListTemplate.transform.parent = roomListContent.parent;
            roomListTemplate.SetActive(false);

            // Set callbacks
            button1vs1.onClick.AddListener(() => {
                if (PhotonNetwork.InLobby)
                    OpenMapSelector(2);
                    //CreateRoom(2);
            });
            button2vs2.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                    OpenMapSelector(4);
                //CreateRoom(4);
            });

            gameObject.SetActive(false);

        }

        private void Update()
        {
          
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // Enable buttons
            EnableButtons(true);

            // Clear room list
            ClearRoomList();

            // Join lobby
            if (Launcher.Instance)
            {
                // Open the connection panel
                if(ConnectionPanel.Instance)
                    ConnectionPanel.Instance.Show(true);

                // Join default lobby
                Launcher.Instance.JoinDefaultLobby();
            }
                
        }

        void OpenMapSelector(int numOfPlayers)
        {
            Debug.Log("Opening map selector...");
            mapSelectorPanel.GetComponent<MapSelectorPanel>().Open(true, numOfPlayers);
            //gameObject.SetActive(false);
        }

        void EnableButtons(bool value)
        {
            button1vs1.interactable = value;
            button2vs2.interactable = value;

        }

        IEnumerator OpenLobbyDelayed()
        {
            yield return new WaitForEndOfFrame();

            lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        void ClearRoomList()
        {

            foreach (GameObject room in rooms)
                Destroy(room);

            rooms.Clear();
        }


        #endregion

        #region public methods

        public void CreateRoom(int maxPlayers, int mapId)
        {
            EnableButtons(false);

            Launcher.Instance.CreateRoom(maxPlayers, mapId);
        }
        #endregion

        #region pun callbacks

        /// <summary>
        /// Entering a room interrupts receiving room list updates from lobbies, so once we exit
        /// we must rejoin the default lobby in order to get the full list again.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            ConnectionPanel.Instance.Show(true);
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            ConnectionPanel.Instance.Show(false);
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            ConnectionPanel.Instance.Show(false);
        }

        public override void OnJoinedRoom()
        {
            // We wait until the frame completed in order to have the player custom properties 
            // set up in the game manager.
            if (!PhotonNetwork.OfflineMode)
                StartCoroutine(OpenLobbyDelayed());
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

        }

        /// <summary>
        /// This methods send back changes on the room list.
        /// It doesn't returns the entire list, but only rooms that have been created or destroyed.
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {

            // We can't simply clear all the rooms, we must check if the room already exists
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible)
                {
                    // If exists remove it
                    GameObject room = rooms.Find(r => r.GetComponent<RoomListElement>().RoomInfo.Name == roomInfo.Name);
                    if (room)
                    {
                        rooms.Remove(room);
                        Destroy(room);
                    }
                }
                else
                {
                    // If doesn't exist then add to the list


                    GameObject room = rooms.Find(r => r.GetComponent<RoomListElement>().RoomInfo.Name == roomInfo.Name);

                    if (!room)
                    {
                        room = GameObject.Instantiate(roomListTemplate, roomListContent, false);

                        room.SetActive(true);
                        room.GetComponent<Button>().onClick.AddListener(() => { Launcher.Instance.JoinRoom(roomInfo.Name); });
                        rooms.Add(room);
                    }

                    // We init the room even if exists in order to update data
                    room.GetComponent<RoomListElement>().Init(roomInfo);
                }
            }


        }

        #endregion



#if OLD
        #region private fields
        [SerializeField]
        Button button1vs1;

        [SerializeField]
        Button button2vs2;


        [SerializeField]
        Button buttonOfflineTraining;


        [SerializeField]
        Transform roomListContent;

        [SerializeField]
        GameObject lobbyPanel;

        //Launcher launcher;
        GameObject roomListTemplate;
        DateTime lastRoomListUpdate;
        float roomListUpdateTime = 2.5f;
        List<GameObject> rooms = new List<GameObject>();
        bool refreshRooms = false;
        bool showLobby = false;
        bool launchTrainingSession = false;
        #endregion

        #region private methods
        private void Start()
        {
            //launcher = FindObjectOfType<Launcher>();

            // Get the room list element template and deactivate it
            roomListTemplate = roomListContent.GetChild(0).gameObject;
            roomListTemplate.transform.parent = roomListContent.parent;
            roomListTemplate.SetActive(false);

            // Set callbacks
            button1vs1.onClick.AddListener(() => {
                if (PhotonNetwork.InLobby)
                    CreateRoom(2);
            } );
            button2vs2.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                    CreateRoom(4);
            });
            buttonOfflineTraining.onClick.AddListener(() => { launchTrainingSession = true; PhotonNetwork.Disconnect(); });


        }

        private void Update()
        {
        
        }

      

        private void LateUpdate()
        {
            //if (showLobby)
            //{
            //    showLobby = false;
            //    // Show lobby
            //    lobbyPanel.SetActive(true);
            //    gameObject.SetActive(false);
            //}
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // Enable buttons
            EnableButtons(true);

            // Clear room list
            ClearRoomList();
         
        }
        

        void ClearRoomList()
        {

            foreach (GameObject room in rooms)
                Destroy(room);

            rooms.Clear();
        }

       

        void EnableButtons(bool value)
        {
            button1vs1.interactable = value;
            button2vs2.interactable = value;

        }

        IEnumerator OpenLobbyDelayed()
        {
            yield return new WaitForEndOfFrame();

            lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    
        #endregion

        #region public methods
        public void CreateRoom(int maxPlayers)
        {
            EnableButtons(false);

            Launcher.Instance.CreateRoom(maxPlayers);
        }
        #endregion

      
        #region pun callbacks
        public override void OnCreatedRoom()
        {

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

            // Show some error message
        }

        public override void OnJoinedRoom()
        {
            // We wait until the frame completed in order to have the player custom properties 
            // set up in the game manager.
            if(!PhotonNetwork.OfflineMode)
                StartCoroutine(OpenLobbyDelayed());
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

        }

        /// <summary>
        /// Entering a room interrupts receiving room list updates from lobbies, so once we exit
        /// we must rejoin the default lobby in order to get the full list again.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// This methods send back changes on the room list.
        /// It doesn't returns the entire list, but only rooms that have been created or destroyed.
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
           
            // We can't simply clear all the rooms, we must check if the room already exists
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible)
                {
                    // If exists remove it
                    GameObject room = rooms.Find(r => r.GetComponent<RoomListElement>().RoomInfo.Name == roomInfo.Name);
                    if(room)
                    {
                        rooms.Remove(room);
                        Destroy(room);
                    }
                }
                else
                {
                    // If doesn't exist then add to the list
                    
                    
                    GameObject room = rooms.Find(r => r.GetComponent<RoomListElement>().RoomInfo.Name == roomInfo.Name);
                    
                    if (!room)
                    {
                        room = GameObject.Instantiate(roomListTemplate, roomListContent, false);
                        
                        room.SetActive(true);
                        room.GetComponent<Button>().onClick.AddListener(() => { Launcher.Instance.JoinRoom(roomInfo.Name); });
                        rooms.Add(room);
                    }
                    
                    // We init the room even if exists in order to update data
                    room.GetComponent<RoomListElement>().Init(roomInfo);
                }
            }
            

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            if (launchTrainingSession)
            {
                launchTrainingSession = false;
                Launcher.Instance.LaunchOffline(2);
            }
        }
        #endregion

#endif
    }


}

