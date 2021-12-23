using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class LobbyPanel : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Button buttonLeaveRoom;

        [SerializeField]
        Button readyButton;

        [SerializeField]
        TMP_Text roomNameField;

        [SerializeField]
        TMP_Text numOfPlayersField;

        [SerializeField]
        GameObject mainPanel;

        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
            buttonLeaveRoom.onClick.AddListener(() => 
            { 
                PhotonNetwork.LeaveRoom(); 
                mainPanel.SetActive(true); 
                gameObject.SetActive(false); 
            });
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (PhotonNetwork.CurrentRoom == null)
                return;

            UpdateRoomNameField();
            UpdateNumOfPlayersField();
        }

        void UpdateNumOfPlayersField()
        {
            if (PhotonNetwork.InRoom)
            {
                numOfPlayersField.text = string.Format("Players: {0}/{1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
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

        #endregion

        #region pun callbacks

        #endregion
    }

}
