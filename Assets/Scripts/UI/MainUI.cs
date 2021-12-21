using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class MainUI : MonoBehaviourPunCallbacks
    {
        #region private fields
        [SerializeField]
        Button button1vs1;

        [SerializeField]
        Button button2vs2;

        [SerializeField]
        Button buttonLeaveRoom;

        [SerializeField]
        TMP_Text roomNameField;

        [SerializeField]
        TMP_Text numOfPlayersField;

        [SerializeField]
        Transform roomListContent;

        [SerializeField]
        Text debugText;

        [SerializeField]
        Button joinLobbyButton;

        #endregion

        #region private fields
        Launcher launcher;
        GameObject roomListTemplate;
        DateTime lastRoomListUpdate;
        float roomListUpdateTime = 2.5f;
        List<GameObject> rooms = new List<GameObject>();
        #endregion

        #region private methods

        private void Awake()
        {
            buttonLeaveRoom.interactable = false;
            
        }

        // Start is called before the first frame update
        void Start()
        {
            launcher = FindObjectOfType<Launcher>();

            // Get the room list element template and deactivate it
            roomListTemplate = roomListContent.GetChild(0).gameObject;
            roomListTemplate.transform.parent = roomListContent.parent;
            roomListTemplate.SetActive(false);

            // Set callbacks
            button1vs1.onClick.AddListener(() => CreateRoom(2));
            button2vs2.onClick.AddListener(() => CreateRoom(4));
            buttonLeaveRoom.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); });
            joinLobbyButton.onClick.AddListener(() => { Launcher.Instance.JoinDefaultLobby(); });


            UpdateRoomNameField();
            UpdateNumOfPlayersField();
            
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRoomList();
        }

        void EnableButtons(bool value)
        {
            button1vs1.interactable = value;
            button2vs2.interactable = value;
        }

        void UpdateNumOfPlayersField()
        {
            if (PhotonNetwork.InRoom)
            {
                numOfPlayersField.text = string.Format("Players: {0}/{1}",PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
            }
            else
            {
                numOfPlayersField.text = string.Format("Players: {0}/{1}", "-", "-");
                
            }
        }

        void UpdateRoomNameField()
        {
            if (PhotonNetwork.InRoom)
            {
                roomNameField.text = PhotonNetwork.CurrentRoom.Name;
            }
            else
            {
                roomNameField.text = "Join or create a room";
                
            }
        
        }

        void ClearRoomList()
        {
          
            foreach (GameObject room in rooms)
                Destroy(room);

            rooms.Clear();
        }

        void UpdateRoomList()
        {
            if ((DateTime.UtcNow - lastRoomListUpdate).TotalSeconds > roomListUpdateTime)
            {
                lastRoomListUpdate = DateTime.UtcNow;

                // Update the room list
                Launcher.Instance.JoinDefaultLobby();
            }
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
            buttonLeaveRoom.interactable = true;

            UpdateRoomNameField();
            UpdateNumOfPlayersField();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

            UpdateRoomNameField();
            UpdateNumOfPlayersField();
        }

        public override void OnJoinedRoom()
        {
            buttonLeaveRoom.interactable = true;

            UpdateRoomNameField();
            UpdateNumOfPlayersField();

            ClearRoomList();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

            UpdateRoomNameField();
            UpdateNumOfPlayersField();
        }

        public override void OnLeftRoom()
        {
           
            // Reset buttons
            EnableButtons(true);

            buttonLeaveRoom.interactable = false;

            UpdateRoomNameField();
            UpdateNumOfPlayersField();

            debugText.text = "Updating roomlist";
            
        }

        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomList();
        
          
            Debug.Log("RoomListUpdated:" + roomList.Count);

          
            debugText.text = "Number of rooms:" + roomList.Count;
            if(roomList.Count > 0)
                debugText.text  += " - RoomName:" + roomList[0].Name;
            
            foreach(RoomInfo roomInfo in roomList)
            {
                GameObject room = GameObject.Instantiate(roomListTemplate, roomListContent, false);
                room.GetComponent<RoomListElement>().Init(roomInfo);
                room.SetActive(true);
                room.GetComponent<Button>().onClick.AddListener(() => { launcher.JoinRoom(roomInfo.Name); });
                rooms.Add(room);
            }

        }

       
        #endregion
    }

}
