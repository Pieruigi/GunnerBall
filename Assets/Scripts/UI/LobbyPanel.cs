using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zoca.Collections;

namespace Zoca.UI
{
    public class LobbyPanel : MonoBehaviourPunCallbacks
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
        Image characterImage;

        [SerializeField]
        Button nextCharacterButton;

        [SerializeField]
        Button prevCharacterButton;

        [SerializeField]
        Image weaponImage;

        [SerializeField]
        GameObject mainPanel;


        List<Character> characters = new List<Character>();
        int currentCharacterId;

        List<Weapon> weapons = new List<Weapon>();
        int currentWeaponId;
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
            buttonLeaveRoom.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); });
            readyButton.onClick.AddListener(() => { GameManager.Instance.SetLocalPlayerReady(true); });

            nextCharacterButton.onClick.AddListener(NextCharacter);
            prevCharacterButton.onClick.AddListener(PrevCharacter);

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (PhotonNetwork.CurrentRoom == null)
            {
                return;
            }
                
            UpdateRoomNameField();
            UpdateNumOfPlayersField();

            // Load characters and weapons
            LoadResources();
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

        void NextCharacter()
        {
            currentCharacterId++;
            if (currentCharacterId == characters.Count)
                currentCharacterId = 0;

            // Set avatar
            characterImage.sprite = characters[currentCharacterId].Avatar;
            // Set character
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId, currentCharacterId);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        void PrevCharacter()
        {
            currentCharacterId--;
            if (currentCharacterId < 0)
                currentCharacterId = characters.Count-1;

            // Set avatar
            characterImage.sprite = characters[currentCharacterId].Avatar;
            // Set character
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId, currentCharacterId);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        

        void LoadResources()
        {
            characters.Clear();
            weapons.Clear();

            // Load characters
            characters = new List<Character>(Resources.LoadAll<Character>(Character.CollectionFolder));

            // Set the default character
            currentCharacterId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId);
            characterImage.sprite = characters[currentCharacterId].Avatar;

            // Load weapons

            // Set default weapon
        }

        #endregion

        #region pun callbacks
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            OnLeftRoom();
        }

        public override void OnLeftRoom()
        {
            mainPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdateNumOfPlayersField();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdateNumOfPlayersField();
        }
        #endregion
    }

}
