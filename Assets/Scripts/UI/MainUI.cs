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
        #endregion

        #region private fields
        Launcher launcher;
        GameObject roomListTemplate;
        DateTime lastRoomListUpdate;
        float roomListUpdateTime = 5;
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
            button1vs1.onClick.AddListener(() => CreateMatch(2));
            button2vs2.onClick.AddListener(() => CreateMatch(4));
            buttonLeaveRoom.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); });

            UpdateRoomNameField();
            UpdateNumOfPlayersField();
            
        }

        // Update is called once per frame
        void Update()
        {
            //UpdateRoomList();
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
                Debug.Log("Reset room name - Not in room.");
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
                Debug.Log("Reset room name - Not in room.");
            }
        
        }

        //void UpdateRoomList()
        //{
        //    if((DateTime.UtcNow - lastRoomListUpdate).TotalSeconds > roomListUpdateTime)
        //    {
        //        lastRoomListUpdate = DateTime.UtcNow;

        //        // Update the room list
                
        //    }
        //}

        #endregion

        #region public methods
        public void CreateMatch(int maxPlayers)
        {
            EnableButtons(false);
           
            launcher.CreateMatch(maxPlayers);
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
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // Update the room list

        }
        #endregion
    }

}
