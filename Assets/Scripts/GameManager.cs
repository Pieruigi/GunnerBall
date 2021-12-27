//#define TEST_SOLO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Zoca.UI;
using UnityEngine.Events;
using Zoca.AI;
using Zoca.Collections;
using System;

namespace Zoca
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region properties

        public static GameManager Instance { get; private set; }

        
        public bool InGame
        {
            get { return inGame; }
        }

        #endregion

        #region private fields
        bool inGame = false;
        float startTime = 25f;
        float startElapsed = 0;
        bool roomIsFull = false;
        bool loading = false;
        #endregion

        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                SceneManager.sceneLoaded += HandleOnSceneLoaded;
                
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
#if FX_DISABLED
        Debug.Log("FxIsDisabled");
#endif
        }

        // Update is called once per frame
        void Update()
        {
            // Check the start timer
            if (PhotonNetwork.IsMasterClient && !inGame && !loading)
            {
                if (roomIsFull)
                {
                    startElapsed += Time.deltaTime;
                    if(startElapsed > startTime)
                    {
                        StartMatch();
                    }
                }
            }
        }

        void StartMatch()
        {
            loading = true;

            InitRoomCustomProperties();

            // Close the room to avoid other players to join
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // The room is full, load the arena
            LoadArena();
        }

        #endregion



        #region private
        /// <summary>
        /// Only used by the master client
        /// </summary>
        void LoadArena()
        {
            string level = "Arena{0}vs{0}";
            if (!PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.LoadLevel(string.Format(level, PhotonNetwork.CurrentRoom.MaxPlayers / 2));
            }
            else
            {
                PhotonNetwork.LoadLevel(string.Format(level, 1));
            }
        }

        void InitRoomCustomProperties()
        {
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, 0f);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)0);
            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
        }

        void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            loading = false;

            if (!scene.name.Equals("MainScene"))
            {
                inGame = true;

                // After the scene has been loaded on this client we can instantiate the networked
                // local player
                if (!PhotonNetwork.OfflineMode) // Online mode
                {
                    Debug.Log("Starting online mode...");

                    if (!PlayerController.LocalPlayer) // Local player is null
                    {
                        // Get the character id set in the lobby ( default character has been set on JoinRoom() )
                        int cId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId);

                        Debug.LogFormat("Loading local player character [CharacterId:{0}].", cId);
                        // Load the character prefab ( the descriptor holding the asset prefab )
                        Character[] collection = Resources.LoadAll<Character>(Character.CollectionFolder);
                        // Get the game asset from the descriptor
                        GameObject playerPrefab = collection[cId].GameAsset;

                        Debug.LogFormat("Character found: {0}", playerPrefab.name);
                        Debug.LogFormat("Spawning {0} on photon network...", playerPrefab.name);

                        // Get the local player team
                        Team team = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);

                        // Get the spawn point depending on the team the player belongs to
                        Transform spawnPoint = null;
                        int teamPlayers = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        if (team == Team.Blue)
                        {
                            int id = actorNumber % teamPlayers;
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[id];
                        }
                        else
                        {
                            int id = actorNumber % (2 * teamPlayers);
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[id];
                        }
                        // Spawn local player on network
                        GameObject p = PhotonNetwork.Instantiate(System.IO.Path.Combine(Character.GameAssetFolder, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                        p.name += "_" + actorNumber;
                    }


                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        // The master client also needs to instantiate the networked ball
                        if (!Ball.Instance)
                        {
                            // For now we only have one ball ( id=0 ) in resources
                            GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Balls)[0].gameObject;
                            PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Balls, ballPrefab.name), LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity);
                            Debug.LogFormat("GameManager - Scene manager: {0}; Ball created:{1}", LevelManager.Instance, Ball.Instance);
                        }

                    }



                }
                else // Offline mode for single player
                {
                    Debug.Log("Starting offline mode...");

                    // Hide cursor
                    Cursor.lockState = CursorLockMode.Locked;

                    // Get the local player character info id
                    int cId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId);

                    // Load descriptor
                    Character[] collection = Resources.LoadAll<Character>(Character.CollectionFolder);
                    // Get prefab from descriptor
                    GameObject playerPrefab = collection[cId].GameAsset;
                    Debug.LogFormat("Character found: {0}", playerPrefab.name);
                    Debug.LogFormat("Spawning {0} on photon network...", playerPrefab.name);
                    // Create the local player
                    Transform spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
                    //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                    PhotonNetwork.Instantiate(System.IO.Path.Combine(Character.GameAssetFolder, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);

                    // Adding local ball
                    if (!Ball.Instance)
                    {
                        // For now we only have one ball ( id=0 ) in resources
                        Debug.Log("Ball Res Folder:" + ResourceFolder.Balls);
                        GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Balls)[0].gameObject;
                        PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Balls, ballPrefab.name), LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity);
                        Debug.LogFormat("GameManager - Scene manager: {0}; Ball created:{1}", LevelManager.Instance, Ball.Instance);
                    }

                   
                    // Spawn the AIs
                    int count = PhotonNetwork.CurrentRoom.MaxPlayers - 1;
                    Team playerTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);
                    Team opponentTeam = playerTeam == Team.Blue ? Team.Red : Team.Blue;
                    int spawnPointId = 1;
                    int aiCharacterId, aiWeaponId;

                    for (int i = 0; i < count; i++)
                    {
                        // Get character and weapon id
                        aiCharacterId = OfflineMatchData.Instance.AiCharacterIds[i];
                        aiWeaponId = OfflineMatchData.Instance.AiWeaponIds[i];

                        // Load avatar
                        collection = Resources.LoadAll<Character>(Character.CollectionFolder);
                        // Get the game asset from the descriptor
                        playerPrefab = collection[aiCharacterId].GameAsset;

                        Player newPlayer = Player.CreateOfflinePlayer(i + 2);
                        PhotonNetwork.CurrentRoom.AddPlayer(newPlayer);

                        // Set character id and weapon id data
                        PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.CharacterId, aiCharacterId);
                        PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.WeaponId, aiWeaponId);

                        if (i + 1 < PhotonNetwork.CurrentRoom.MaxPlayers / 2)
                        {
                            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor, playerTeam);
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[spawnPointId];
                        }
                        else
                        {
                            if (i + 1 == PhotonNetwork.CurrentRoom.MaxPlayers / 2)
                                spawnPointId = 0; // Reset the spawn point id

                            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor, opponentTeam);
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[spawnPointId];
                        }

                        spawnPointId++;

                        GameObject newPlayerObject = PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(Character.GameAssetFolder, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                        newPlayerObject.GetComponent<PlayerAI>().Activate();
                        newPlayerObject.GetComponent<PlayerAI>().Team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor);
                        newPlayerObject.GetComponent<PhotonView>().OwnerActorNr = i + 2;


                    }

                    Debug.Log("Players.Count: " + PhotonNetwork.CurrentRoom.Players.Count);
                    Debug.LogFormat("OfflinePlayer: {0}, team:{1}", PhotonNetwork.CurrentRoom.Players[1].NickName, PhotonNetwork.CurrentRoom.Players[1].CustomProperties[PlayerCustomPropertyKey.TeamColor]);
                    Debug.LogFormat("OfflinePlayer: {0}, team:{1}", PhotonNetwork.CurrentRoom.Players[2].NickName, PhotonNetwork.CurrentRoom.Players[2].CustomProperties[PlayerCustomPropertyKey.TeamColor]);


                }



            }
            else
            {
                inGame = false;

                // The player camera has not been destroyed leaving the arena, so we do it here.
                //Debug.LogFormat("GameManager - Not in game, camera instance: {0}", PlayerCamera.Instance);
                //if (PlayerCamera.Instance)
                //    Destroy(PlayerCamera.Instance.gameObject);

                // Destroy player if exists
                if (PlayerController.LocalPlayer)
                    Destroy(PlayerController.LocalPlayer);

            }
            Debug.LogFormat("GameManager - scene loaded [Name:{0}]; inGame:{1}", scene.name, inGame);
        }


        #endregion

        #region callbacks

        /// <summary>
        /// Called local when player enters the room
        /// </summary>
        public override void OnJoinedRoom()
        {


            // Try to set the start timer
            if (PhotonNetwork.IsMasterClient && !inGame)
            {
                startElapsed = 0;
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    roomIsFull = true;
                }
            }
            

            Debug.LogFormat("PUN - local player actor number: {0}", PhotonNetwork.LocalPlayer.ActorNumber);
            // Reset ready flag
            SetLocalPlayerReady(false);

            //if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            if (PhotonNetwork.LocalPlayer.ActorNumber <= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            {
                //PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Blue);
                PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor, Team.Blue);
            }
            else
            {
                //PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Red);
                PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor, Team.Red);
            }

            int localCharacterId = 0;
            int localWeaponId = 0;
            if (PhotonNetwork.OfflineMode)
            {
                // Use the OfflineMatchData structure
                localCharacterId = OfflineMatchData.Instance.LocalPlayerCharacterId;
                localWeaponId = OfflineMatchData.Instance.LocalPlayerWeaponId;
            }

            // Set the default character
            Debug.LogFormat("PUN - Setting default character id.");
            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, localCharacterId);
            // Set weapon 
            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.WeaponId, localWeaponId);
            // Save player properties
            PlayerCustomPropertyUtility.SynchronizePlayerCustomProperties(PhotonNetwork.LocalPlayer);

            // Offline mode
            if (PhotonNetwork.OfflineMode)
            {
                InitRoomCustomProperties();

                // Close the room to avoid other players to join our test 
                PhotonNetwork.CurrentRoom.IsOpen = false;

                // The room is not full but we want to test it
                LoadArena();
            }



        }

        /// <summary>
        /// Called when another player enters the room; 
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Try to set the start timer
            if (PhotonNetwork.IsMasterClient && !inGame)
            {
                startElapsed = 0;
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    roomIsFull = true;
                }
                    
            }

            Debug.LogFormat("PUN - New player [ID:{0}] entered the room [Name:{1}].", newPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
            // We don't need to reload the arena if we are already playing; this may happen
            // when game has started and a player disconnects and then connects again.
            if (!GameManager.Instance.inGame)
            {
                // Only the master client can load the arena
                if (PhotonNetwork.IsMasterClient)
                {

                    Debug.LogFormat("PUN - IsMasterClient: {0}", PhotonNetwork.IsMasterClient);
                    Debug.LogFormat("PUN - Current room max players: {0}", PhotonNetwork.CurrentRoom.MaxPlayers);
                    Debug.LogFormat("PUN - Current room current players: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
                    //if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                   
                    //{
                        
                        //InitRoomCustomProperties();

                        //// Close the room to avoid other players to join
                        //PhotonNetwork.CurrentRoom.IsOpen = false;
                        //PhotonNetwork.CurrentRoom.IsVisible = false;
                                             
                        //// The room is full, load the arena
                        //LoadArena();

                        
                    //}

                }

            }
           
        }

        /// <summary>
        /// Called when another player leaves
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // Reset timer
            if (PhotonNetwork.IsMasterClient && !inGame)
            {
                roomIsFull = false;
            }
            Debug.LogFormat("PUN - Player [ID:{0}] left room [Name:{1}].", otherPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
        }

        /// <summary>
        /// Called when the local player leaves the room
        /// </summary>
        public override void OnLeftRoom()
        {
            // Reset timer
            if (PhotonNetwork.IsMasterClient && !inGame)
            {
                roomIsFull = false;
            }

            Debug.LogFormat("PUN - Left room.");
            if (inGame)
                SceneManager.LoadScene("MainScene");
        }


        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {

        }

        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerCustomPropertyKey.Ready))
            {
                if (!GameManager.Instance.InGame) 
                {
                    // Only the master client can start the game
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // If the room is full and all the players are ready then starts the match
                        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.Players.Count)
                        {
                            bool start = true;
                            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                            {
                                // If there is at least one player not ready then return.
                                if (!IsPlayerReady(player))
                                {
                                    start = false;
                                    break;
                                }

                            }

                            if (start)
                            {
                                // Start
                                StartMatch();
                            }
                        }
                    }

                        
                }

                
               
            }


        }


       
        #endregion

        #region public methods

        public void Pause()
        {
            if (inGame)
            {
                // We should open some kind of game menu here
                PhotonNetwork.LeaveRoom();
            }

        }

        public void LeaveRoom()
        {
            if (inGame)
            {
                Debug.Log("Leaving room...");
                PhotonNetwork.LeaveRoom();
            }

        }
      

        public void SetLocalPlayerReady(bool value)
        {
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.Ready, value ? (byte)1:(byte)0);
            PlayerCustomPropertyUtility.SynchronizeLocalPlayerCustomProperties();
        }

        public bool IsPlayerReady(Player player)
        {
            object b = 0; 
            if(PlayerCustomPropertyUtility.TryGetPlayerCustomProperty(player, PlayerCustomPropertyKey.Ready, out b))
            {
                //Debug.LogFormat("Player {0} is ready: {1}", player.ActorNumber, (byte)b == 0 ? false : true);
                return (byte)b == 0 ? false : true;
            }
            else
            {
                //Debug.LogFormat("Player {0} is ready: {1}", player.ActorNumber, false);
                return false;
            }
            
        }

        public bool IsLocalPlayerReady()
        {
            return IsPlayerReady(PhotonNetwork.LocalPlayer);
        }

        #endregion
    }

}
