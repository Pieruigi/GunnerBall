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
        Transform roomListContent;

        [SerializeField]
        GameObject lobbyPanel;

        Launcher launcher;
        GameObject roomListTemplate;
        DateTime lastRoomListUpdate;
        float roomListUpdateTime = 2.5f;
        List<GameObject> rooms = new List<GameObject>();
        #endregion

        #region private methods
        private void Start()
        {
            launcher = FindObjectOfType<Launcher>();

            // Get the room list element template and deactivate it
            roomListTemplate = roomListContent.GetChild(0).gameObject;
            roomListTemplate.transform.parent = roomListContent.parent;
            roomListTemplate.SetActive(false);

            // Set callbacks
            button1vs1.onClick.AddListener(() => CreateRoom(2));
            button2vs2.onClick.AddListener(() => CreateRoom(4));
            
        }

        private void Update()
        {
        
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
        #endregion

        #region public methods
        public void CreateRoom(int maxPlayers)
        {
            EnableButtons(false);

            launcher.CreateRoom(maxPlayers);
        }
        #endregion

      
        #region pun callbacks
        public override void OnCreatedRoom()
        {
            // Show lobby
            lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

            // Show some error message
        }

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
                    Debug.Log("Checking for room:" + roomInfo.Name);
                    // If doesn't exist then add to the list
                    Debug.Log("Rooms.Count:" + rooms.Count);
                    if(rooms.Count > 0)
                        Debug.Log("Rooms.Count:" + rooms[0].GetComponent<RoomListElement>());

                    GameObject room = rooms.Find(r => r.GetComponent<RoomListElement>().RoomInfo.Name == roomInfo.Name);
                    Debug.Log("Room" + room);
                    if (!room)
                    {
                        Debug.Log("Room not found:" + roomInfo.Name);
                        room = GameObject.Instantiate(roomListTemplate, roomListContent, false);
                        room.GetComponent<RoomListElement>().Init(roomInfo);
                        Debug.Log("Init RoomInfo:" + roomInfo.Name);
                        room.SetActive(true);
                        room.GetComponent<Button>().onClick.AddListener(() => { launcher.JoinRoom(roomInfo.Name); });
                        rooms.Add(room);
                    }
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
