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
        Button nextWeaponButton;

        [SerializeField]
        Button prevWeaponButton;

        [SerializeField]
        Image weaponImage;

        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        TMP_Text testPlayerName;

        [SerializeField]
        Image testPlayerAvatar;


        List<Character> characters = new List<Character>();
        int currentCharacterId;

        //List<Weapon> weapons = new List<Weapon>();
        int currentWeaponId;
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
            buttonLeaveRoom.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); });
            readyButton.onClick.AddListener(() => { GameManager.Instance.SetLocalPlayerReady(true); });

            // Add character button listener
            nextCharacterButton.onClick.AddListener(NextCharacter);
            prevCharacterButton.onClick.AddListener(PrevCharacter);

            // Add weapon button listener
            nextWeaponButton.onClick.AddListener(NextWeapon);
            prevWeaponButton.onClick.AddListener(PrevWeapon);

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
                //roomNameField.text = PhotonNetwork.CurrentRoom.Name;
                roomNameField.text = (string)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.PlayerCreator);
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

            // Reset weapons
            currentWeaponId = 0;
            weaponImage.sprite = characters[currentCharacterId].Weapons[currentWeaponId].Icon;


            // Set character
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.WeaponId, currentWeaponId);
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

            // Reset weapons
            currentWeaponId = 0;
            weaponImage.sprite = characters[currentCharacterId].Weapons[currentWeaponId].Icon;


            // Set character
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.WeaponId, currentWeaponId);
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId, currentCharacterId);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        void NextWeapon()
        {
            // Move to the next weapon
            currentWeaponId++;
            if (currentWeaponId == characters[currentCharacterId].Weapons.Count)
                currentWeaponId = 0;

            // Set the new icon
            weaponImage.sprite = characters[currentCharacterId].Weapons[currentWeaponId].Icon;

            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.WeaponId, currentWeaponId);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        void PrevWeapon()
        {
            // Move to the next weapon
            currentWeaponId--;
            if (currentWeaponId < 0)
                currentWeaponId = characters[currentCharacterId].Weapons.Count-1;

            // Set the new icon
            weaponImage.sprite = characters[currentCharacterId].Weapons[currentWeaponId].Icon;

            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.WeaponId, currentWeaponId);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        void LoadResources()
        {
            characters.Clear();
            
            // Load characters
            characters = new List<Character>(Resources.LoadAll<Character>(Character.CollectionFolder));

            // Set the default character
            currentCharacterId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId);
            characterImage.sprite = characters[currentCharacterId].Avatar;

            // Load weapons
            currentWeaponId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.WeaponId);
            weaponImage.sprite = characters[currentWeaponId].Weapons[currentWeaponId].Icon;

            
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

            // Test name and avatar
            testPlayerName.text = otherPlayer.NickName;
        }
        #endregion
    }

}
