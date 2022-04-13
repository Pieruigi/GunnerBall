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
using Steamworks;

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

        public bool Leaving { get; set; }
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
            //if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && !inGame && !loading)
            //{

            //    //if (roomIsFull)
            //    if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            //    {
            //        startElapsed += Time.deltaTime;
            //        if(startElapsed > startTime)
            //        {
            //            StartMatch();
            //        }
            //    }
            //}
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
            // Get the map id from the custom properties
            int mapId = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MapId);

            string mapName = MapManager.Instance.GetMap(mapId).Name;

          
            StartCoroutine(LoadArenaCoroutine(mapName));

        }

        IEnumerator LoadArenaCoroutine(string mapName)
        {
            yield return new WaitForSeconds(1f);

            if(Leaving)
            {
                Leaving = false;
                yield break;
            }

            //string level = "Arena{0}vs{0}";
            if (!PhotonNetwork.OfflineMode)
            {
                if (AllPlayersAreReady())
                {
                    PhotonNetwork.LoadLevel(string.Format(mapName, 1));
                }
                else
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                    PhotonNetwork.CurrentRoom.IsVisible = true;
                }
            }
            else
            {
                if (AllPlayersAreReady())
                {
                    PhotonNetwork.LoadLevel(string.Format(mapName, 1));
                }
                else 
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                    PhotonNetwork.CurrentRoom.IsVisible = true;
                }
            }
        }

        bool IsBlueTeamFull()
        {
            
            int count = 0;
            foreach(Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (p == PhotonNetwork.LocalPlayer)
                    continue;

                if(Team.Blue == (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(p, PlayerCustomPropertyKey.TeamColor))
                {
                    count++;
                }
            }

            if (count < PhotonNetwork.CurrentRoom.MaxPlayers / 2)
                return false;

            return true;
        }

        void InitRoomCustomProperties()
        {
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, 0f);
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)0);
            //RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

            ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
            ht.Add(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
            ht.Add(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
            ht.Add(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
            ht.Add(RoomCustomPropertyKey.MatchTimeElapsed, (float)0f);
            ht.Add(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
            ht.Add(RoomCustomPropertyKey.RedTeamScore, (byte)0);
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
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
                        int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                        // Get the other player
                        //Player other = null;
                        int localSpawnId = 0;
                        foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys)
                        {
                            if (PhotonNetwork.CurrentRoom.Players[key] == PhotonNetwork.LocalPlayer)
                                continue;

                            Team t = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(PhotonNetwork.CurrentRoom.Players[key], PlayerCustomPropertyKey.TeamColor);
                            if (t == team)
                            {
                                //other = PhotonNetwork.CurrentRoom.Players[key];
                                if (PhotonNetwork.CurrentRoom.Players[key].ActorNumber > localActorNumber)
                                    localSpawnId++;
                                //break;
                            };

                        }

                        if (team == Team.Blue)
                        {

                            //int id = actorNumber % teamPlayers;
                            //int id = 0;
                            //if (other != null && localActorNumber > other.ActorNumber)
                            //    id = 1;
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[localSpawnId];
                        }
                        else
                        {
                            //int id = actorNumber % (2 * teamPlayers);
                            //int id = 0;
                            //if (other != null && localActorNumber > other.ActorNumber)
                            //    id = 1;
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[localSpawnId];
                        }
                        // Spawn local player on network
                        GameObject p = PhotonNetwork.Instantiate(System.IO.Path.Combine(Character.GameAssetFolder, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                        p.name += "_" + localActorNumber;
                    }


                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        // The master client also needs to instantiate the networked ball
                        if (!Ball.Instance)
                        {
                            // Get the map id from the custom properties
                            int mapId = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MapId);

                            Map map = MapManager.Instance.GetMap(mapId);

                            // For now we only have one ball ( id=0 ) in resources
                            //GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Balls)[0].gameObject;
                            GameObject ballPrefab = map.Ball;
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
                        // Get the map id from the custom properties
                        int mapId = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MapId);

                        Map map = MapManager.Instance.GetMap(mapId);

                        // For now we only have one ball ( id=0 ) in resources
                        Debug.Log("Ball Res Folder:" + ResourceFolder.Balls);
                        //GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Balls)[1].gameObject;
                        GameObject ballPrefab = map.Ball;
                        PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Balls, ballPrefab.name), LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity);
                        Debug.LogFormat("GameManager - Scene manager: {0}; Ball created:{1}", LevelManager.Instance, Ball.Instance);
                    }


                
                    int count = PhotonNetwork.CurrentRoom.MaxPlayers;

                    // Get the player 

                    int blueSpawnId = 1;
                    int redSpawnId = 0;
                    for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
                    {
                        Player p = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values)[i];
                        if (p == PhotonNetwork.LocalPlayer)
                            continue;

                        // Spawn the ai
                        Team aiTeam = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(p, PlayerCustomPropertyKey.TeamColor);
                        int aiCharacterId = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(p, PlayerCustomPropertyKey.CharacterId);
                        int aiWeaponInd = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(p, PlayerCustomPropertyKey.WeaponId);

                        // Load avatar
                        collection = Resources.LoadAll<Character>(Character.CollectionFolder);
                        // Get the game asset from the descriptor
                        playerPrefab = collection[aiCharacterId].GameAsset;

                        int spawnPointId = 0;
                        // Get the spawn point
                        if (aiTeam == Team.Blue)
                        {
                            spawnPointId = blueSpawnId;
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[spawnPointId];
                        }
                        else
                        {
                            spawnPointId = redSpawnId;
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[spawnPointId];
                            redSpawnId++;
                        }


                        GameObject newPlayerObject = PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(Character.GameAssetFolder, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                        newPlayerObject.GetComponent<PlayerAI>().Activate();
                        newPlayerObject.GetComponent<PlayerAI>().Team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(p, PlayerCustomPropertyKey.TeamColor);
                        newPlayerObject.GetComponent<PhotonView>().OwnerActorNr = p.ActorNumber;
                        // Deactivate the ai to let the local player train alone
                        newPlayerObject.SetActive(false);
                    }


                }
            }
            else // Not in game
            {
                inGame = false;

                Cursor.lockState = CursorLockMode.None;
                // The player camera has not been destroyed leaving the arena, so we do it here.
                //Debug.LogFormat("GameManager - Not in game, camera instance: {0}", PlayerCamera.Instance);
                //if (PlayerCamera.Instance)
                //    Destroy(PlayerCamera.Instance.gameObject);

                // Destroy player if exists
                if (PlayerController.LocalPlayer)
                    Destroy(PlayerController.LocalPlayer);

                // Destroy player camera
                GameObject camera = GameObject.FindGameObjectWithTag(Tag.PlayerCamera);
                if (camera)
                    Destroy(camera);

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

#if !DISABLESTEAMWORKS// && !UNITY_EDITOR
            // Set the user id
            Debug.Log("Setting user id...:"+SteamUser.GetSteamID().m_SteamID);
            PlayerCustomPropertyUtility.AddOrUpdateLocalPlayerCustomProperty(PlayerCustomPropertyKey.UserId, (long)SteamUser.GetSteamID().m_SteamID);
            Debug.Log("User id set");
#endif

            
            if (!IsBlueTeamFull())
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
            //if (PhotonNetwork.OfflineMode)
            //{
            //    // Use the OfflineMatchData structure
            //    localCharacterId = OfflineMatchData.Instance.LocalPlayerCharacterId;
            //    localWeaponId = OfflineMatchData.Instance.LocalPlayerWeaponId;
            //}

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
                //LoadArena();


                ///////////////////////////////////////////////////////////
                ///
                // Spawn the AIs

                Character[] characters = Resources.LoadAll<Character>(Character.CollectionFolder);

                int count = PhotonNetwork.CurrentRoom.MaxPlayers - 1;
                Team playerTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);
                Team opponentTeam = playerTeam == Team.Blue ? Team.Red : Team.Blue;
                int aiCharacterId = 0, aiWeaponId = 0;

                for (int i = 0; i < count; i++)
                {
                    // Get character and weapon id
                    Player newPlayer = Player.CreateOfflinePlayer(i + 2);
                    PhotonNetwork.CurrentRoom.AddPlayer(newPlayer);

                    // Set character id and weapon id data
                    aiCharacterId = UnityEngine.Random.Range(0, characters.Length);
                    aiWeaponId = UnityEngine.Random.Range(0, characters[aiCharacterId].Weapons.Count);
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.CharacterId, aiCharacterId);
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.WeaponId, aiWeaponId);

                    // Set team
                    if (i + 1 < PhotonNetwork.CurrentRoom.MaxPlayers / 2)
                        PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor, playerTeam);
                    else
                        PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor, opponentTeam);


                    /////////////////////////////////////////////////////////////

                }



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
                        if (!PhotonNetwork.OfflineMode)
                        {
                            // If the room is full and all the players are ready then starts the match
                            if (AllPlayersAreReady())
                            {
                                StartMatch();
                            }
                            //if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.Players.Count)
                            //{
                            //    bool start = true;
                            //    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                            //    {
                            //        // If there is at least one player not ready then return.
                            //        if (!IsPlayerReady(player))
                            //        {
                            //            start = false;
                            //            break;
                            //        }

                            //    }

                            //    if (start)
                            //    {
                            //        // Start
                            //        StartMatch();
                            //    }
                            //}
                        }
                        else // Offline mode
                        {
                            // Only need the local player to be ready
                            if (IsLocalPlayerReady())
                                StartMatch();
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
            //// In offline mode all the AIs are ready
            //if(PhotonNetwork.OfflineMode && player != PhotonNetwork.LocalPlayer)
            //    return true;
                
            
            object b = 0;
            if (PlayerCustomPropertyUtility.TryGetPlayerCustomProperty(player, PlayerCustomPropertyKey.Ready, out b))
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

        public bool AllPlayersAreReady()
        {
            if (PhotonNetwork.OfflineMode && IsLocalPlayerReady())
                return true;

            // If the room is full and all the players are ready then starts the match
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.Players.Count)
            {
                foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    // If there is at least one player not ready then return.
                    if (!IsPlayerReady(player))
                    {
                        //start = false;
                        return false;
                    }

                }

                return true;
            }

            return false;
        }

        #endregion
    }

}
