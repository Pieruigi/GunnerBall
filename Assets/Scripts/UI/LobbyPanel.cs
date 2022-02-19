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
        [Header("Panel Detail")]
        [SerializeField]
        Button buttonLeaveRoom;

        [SerializeField]
        Button readyButton;

        [SerializeField]
        GameObject mainPanel;

        [Header("Room Detail")]
        [SerializeField]
        TMP_Text roomNameField;

        [SerializeField]
        TMP_Text numOfPlayersField;

        [SerializeField]
        List<Image> teamObjectImages;

        [SerializeField]
        Image arenaImage;

        [SerializeField]
        GameObject blueTeamPanel;

        [SerializeField]
        GameObject redTeamPanel;

        [SerializeField]
        Color blueTeamColor;

        [SerializeField]
        Color redTeamColor;

        [Header("Character Detail")]
        [SerializeField]
        Image characterImage;

        [SerializeField]
        Button nextCharacterButton;

        [SerializeField]
        Button prevCharacterButton;

        [SerializeField]
        TMP_Text characterSpeedText;

        [SerializeField]
        TMP_Text characterStaminaText;

        [SerializeField]
        TMP_Text characterStunnedText;

        [Header("Weapon Detail")]
        [SerializeField]
        Button nextWeaponButton;

        [SerializeField]
        Button prevWeaponButton;

        [SerializeField]
        Image weaponImage;

        [SerializeField]
        TMP_Text firePowerText;

        [SerializeField]
        TMP_Text fireRateText;

        [SerializeField]
        TMP_Text fireRangeText;





        //[SerializeField]
        //TMP_Text testPlayerName;

        //[SerializeField]
        //Image testPlayerAvatar;


        List<Character> characters = new List<Character>();
        int currentCharacterId;

        //List<Weapon> weapons = new List<Weapon>();
        int currentWeaponId;

        List<LobbyPlayer> bluePlayers;
        List<LobbyPlayer> redPlayers;

        //bool offline = false;
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

            // Init player lists
            bluePlayers = new List<LobbyPlayer>(blueTeamPanel.GetComponentsInChildren<LobbyPlayer>());
            redPlayers = new List<LobbyPlayer>(redTeamPanel.GetComponentsInChildren<LobbyPlayer>());
            
            // Deactivate
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

            // Init red and blue teams
            ResetTeams();

            Debug.Log("Num of players:" + PhotonNetwork.CurrentRoom.PlayerCount);

            // Set teams
            if (!PhotonNetwork.OfflineMode)
            {
                foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    SetTeamPlayer(p);
                }
            }
            else
            {
                // Set the local player
                SetTeamPlayer(PhotonNetwork.LocalPlayer);

                // In the game manager we simply deactivate the ai, so in this case the game
                // is an offline 1vs1 match and we need to reset the empy red team player
                redPlayers[0].Lock();
            }
            

            // Colorize
            Team team = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);
            Color c = team == Team.Blue ? blueTeamColor : redTeamColor;
            foreach(Image image in teamObjectImages)
            {
                image.color = c;
            }

            // Set the arena image
            // Get the arena id
            int mapId = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MapId);
            arenaImage.sprite = MapManager.Instance.GetMap(mapId).ImageSprite;

            // Load characters and weapons
            LoadResources();
        }
        
        void ResetTeams()
        {
            // Get the max number of players
            int count = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
            // Init each team list
            for (int i = 0; i < bluePlayers.Count; i++)
            {
                if (i >= count)
                {
                    // Lock both blue and red players
                    bluePlayers[i].Lock();
                    redPlayers[i].Lock();
                }
                else
                {
                    // Clear both blue and red players
                    bluePlayers[i].Clear();
                    redPlayers[i].Clear();
                }

            }
        }

        void SetTeamPlayer(Player player)
        {
            // Get the team of the player
            Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(player, PlayerCustomPropertyKey.TeamColor);
            LobbyPlayer lp = team == Team.Blue ? bluePlayers.Find(p => p.IsEmpty && !p.IsLocked) : redPlayers.Find(p => p.IsEmpty && !p.IsLocked);
            
            // It shouldn't happen this
            if (lp == null)
            {
                Debug.LogErrorFormat("No room for the player...");
                return;
            }
            // Add the new player
            lp.Init(player);
        }

        IEnumerator SetTeamPlayerDelayed(Player player)
        {
            yield return new WaitForSeconds(1);
            bool found = false;
            foreach(Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if(p == player)
                {
                    found = true;
                    break;
                }
            }
            if(found)
                SetTeamPlayer(player);
        }

        /// <summary>
        /// Clear the corresponding lobby player
        /// </summary>
        /// <param name="p"></param>
        void ResetTeamPlayer(Player player)
        {
            // Get the corresponding lobby player element
            LobbyPlayer lp = bluePlayers.Find(p => p.Player == player);
            if (!lp)
                lp = redPlayers.Find(p => p.Player == player);

            // Clear
            lp.Clear();
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
            SetWeaponStats(characters[currentCharacterId].Weapons[currentWeaponId]);
            SetCharacterStats(characters[currentCharacterId]);

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
            SetWeaponStats(characters[currentCharacterId].Weapons[currentWeaponId]);
            SetCharacterStats(characters[currentCharacterId]);

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
            SetWeaponStats(characters[currentCharacterId].Weapons[currentWeaponId]);
            SetCharacterStats(characters[currentCharacterId]);

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
            SetWeaponStats(characters[currentCharacterId].Weapons[currentWeaponId]);
            SetCharacterStats(characters[currentCharacterId]);

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
            weaponImage.sprite = characters[currentCharacterId].Weapons[currentWeaponId].Icon;

            SetWeaponStats(characters[currentCharacterId].Weapons[currentWeaponId]);
            SetCharacterStats(characters[currentCharacterId]);
        }

        void SetWeaponStats(Weapon weapon)
        {
            firePowerText.text = weapon.FirePower.ToString();
            fireRateText.text = weapon.FireRate.ToString();
            fireRangeText.text = weapon.FireRange.ToString();
        } 

        void SetCharacterStats(Character character)
        {
            characterSpeedText.text = character.Speed.ToString();
            characterStaminaText.text = character.Stamina.ToString();
            characterStunnedText.text = character.FreezingCooldown.ToString();
        }

        #endregion

        #region pun callbacks

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            OnLeftRoom();
        }

        public override void OnLeftRoom()
        {
            // Reset the lobby team
            ResetTeamPlayer(PhotonNetwork.LocalPlayer);

            mainPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        
        

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Set the new player 
            //SetTeamPlayer(newPlayer);
            StartCoroutine(SetTeamPlayerDelayed(newPlayer));

            // Update number of players
            UpdateNumOfPlayersField();

//            // Test name and avatar
//            testPlayerName.text = newPlayer.NickName;

//            // Get the user id
//#if !DISABLESTEAMWORKS
//            Debug.Log("Getting user id...");
//            long userId = (long)PlayerCustomPropertyUtility.GetPlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.UserId);
//            Debug.Log("User id: " + userId);
//            Texture2D avatar;
//            if(SteamUtility.TryGetPlayerAvatarAsTexture2D((ulong)userId, out avatar))
//            {
//                testPlayerAvatar.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), Vector2.zero);
//            }
//#endif
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // Reset the corresponding lobby player
            ResetTeamPlayer(otherPlayer);

            UpdateNumOfPlayersField();

            // Test name and avatar
            //testPlayerName.text = "Empty";
        }
        #endregion
    }

}
