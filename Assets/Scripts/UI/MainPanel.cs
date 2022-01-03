using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class MainPanel: MonoBehaviourPunCallbacks
    {

        #region private fields
        [SerializeField]
        Button button1vs1;

        [SerializeField]
        Button button2vs2;


        [SerializeField]
        Button buttonTestOffline;


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
            
#if UNITY_EDITOR
            buttonTestOffline.onClick.AddListener(() => { PhotonNetwork.Disconnect(); Launcher.Instance.LaunchOffline(2); });
#else
            Destroy(buttonTestOffline.gameObject);
#endif

        }

        private void Update()
        {
        
        }

      

        private void LateUpdate()
        {
            if (showLobby)
            {
                showLobby = false;
                // Show lobby
                lobbyPanel.SetActive(true);
                gameObject.SetActive(false);
            }
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
            //showLobby = true;
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
        #endregion
    }


}

#if OLD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> popUpObjects;

        float showTime;

        private void Awake()
        {
            // Set the cursor visible 
            Cursor.lockState = CursorLockMode.None;

            ShowObjects(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(showTime > 0)
            {
                showTime -= Time.deltaTime;

                // Not yet
                if (showTime > 0)
                    return;


                // Show or hide button
                ShowObjects(true);

            }
        }

        private void OnEnable()
        {
            showTime = 1;
        }

        private void OnDisable()
        {
            showTime = 0;
            ShowObjects(false);
        }


        void ShowObjects(bool value)
        {
            foreach (GameObject o in popUpObjects)
            {
                o.SetActive(value);
            }
        }
    }

}
#endif
