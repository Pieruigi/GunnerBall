//#define TEST_SINGLE_PLAYER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Zoca.UI;
using UnityEngine.Events;
using Zoca.AI;

namespace Zoca
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        

        public static GameManager Instance { get; private set; }

        bool inGame = false;
        public bool InGame
        {
            get { return inGame; }
        }

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
            
        }

        

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

        

        public void Resume()
        {
            
        }

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

        #endregion

#region callbacks
#if !TEST_SINGLE_PLAYER
        /// <summary>
        /// Called local when player enters the room
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("PUN - local player actor number: {0}", PhotonNetwork.LocalPlayer.ActorNumber);
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

            // Set the default character
            Debug.LogFormat("PUN - Setting default character id.");
            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, 0);
            PlayerCustomPropertyUtility.SynchronizePlayerCustomProperties(PhotonNetwork.LocalPlayer);

            if (PhotonNetwork.OfflineMode)
            {
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, 0f);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)0);
                RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();


                // Close the room to avoid other players to join our test 
                PhotonNetwork.CurrentRoom.IsOpen = false;

                // The room is not full but we want to test it
                LoadArena();
            }
            //else
            //{
            //    RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
            //}
        }
#endif
        /// <summary>
        /// Called when another player enters the room; 
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
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
                    if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                    //if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                    {
                        // A match starting time is needed to set countdown
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, 0f);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)0);
                        RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

                        // Close the room to avoid other players to join
                        PhotonNetwork.CurrentRoom.IsOpen = false;

                        //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.StartTime] = (float)PhotonNetwork.Time;
                        //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Starting;
                        //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchElapsed] = 0f;

                        //PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                        // The room is full, load the arena
                        LoadArena();

                        
                    }

                }

            }
           
        }

        /// <summary>
        /// Called when another player leaves
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("PUN - Player [ID:{0}] left room [Name:{1}].", otherPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
        }

#if TEST_SINGLE_PLAYER && UNITY_EDITOR
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("PUN - local player actor number: {0}", PhotonNetwork.LocalPlayer.ActorNumber);
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

            // Set the default character
            Debug.LogFormat("PUN - Setting default character id.");
            PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, 0);
            //PlayerCustomPropertyUtility.SynchronizePlayerCustomProperties(PhotonNetwork.LocalPlayer);

            // A match starting time is needed to set countdown
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)MatchState.None);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, 0f);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)0);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)0);
            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

            //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.StartTime] = (float)PhotonNetwork.Time;
            //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Starting;
            //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchElapsed] = 0f;

            // Close the room to avoid other players to join our test 
            PhotonNetwork.CurrentRoom.IsOpen = false;

            // The room is not full but we want to test it
            LoadArena();
        }
#endif

        /// <summary>
        /// Called when the local player leaves the room
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.LogFormat("PUN - Left room.");
            SceneManager.LoadScene("MainScene");
        }

       

        void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
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
                        
                        int cId = (int)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.CharacterId);
                        //cId++; // To load prototype
                        //if (!PlayerCustomPropertyUtility.TryGetPlayerCustomProperty<int>(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, ref cId))
                        //{
                        //    Debug.LogErrorFormat("GameManager - Empty property for local player: [{0}]", PlayerCustomPropertyKey.CharacterId);
                        //}
                        Debug.LogFormat("Loading local player character [CharacterId:{0}].", cId);
                        GameObject playerPrefab = Resources.LoadAll<PlayerController>(ResourceFolder.Characters)[(int)cId].gameObject;
                        Debug.LogFormat("Character found: {0}", playerPrefab.name);
                        Debug.LogFormat("Spawning {0} on photon network...", playerPrefab.name);

                        // Spawn the networked local player ( only support 1vs1 at the moment )
                        // Get the player team
                        
                        Team team = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);
                        //if (!PlayerCustomPropertyUtility.TryGetPlayerCustomProperty<Team>(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, ref team))
                        //{
                        //    Debug.LogErrorFormat("PlayerController - property is empty: {0}", PlayerCustomPropertyKey.TeamColor);
                        //}

                        // Get the spawn point depending on the team the player belongs to
                        Transform spawnPoint = null;
                        int teamPlayers = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
                        //int actorNumber = PlayerController.Local.photonView.Owner.ActorNumber;
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        if (team == Team.Blue)
                        {
                            int id = actorNumber % teamPlayers;
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[id];
                        }
                        else
                        {
                            int id = actorNumber % (2*teamPlayers);
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[id];
                        }
                        // Spawn local player
                        GameObject p = PhotonNetwork.Instantiate(System.IO.Path.Combine(ResourceFolder.Characters, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
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

                    // Add default custom properties
                    int cId = 1;
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Blue);
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, cId);
                    // Get character resource
                    
                    GameObject playerPrefab = Resources.LoadAll<PlayerController>(ResourceFolder.Characters)[cId].gameObject;
                    Debug.LogFormat("Character found: {0}", playerPrefab.name);
                    Debug.LogFormat("Spawning {0} on photon network...", playerPrefab.name);
                    // Create local player
                    Transform spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
                    //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                    PhotonNetwork.Instantiate(System.IO.Path.Combine(ResourceFolder.Characters, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);

                    // Adding local ball
                    if (!Ball.Instance)
                    {
                        // For now we only have one ball ( id=0 ) in resources
                        GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Balls)[0].gameObject;
                        PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Balls, ballPrefab.name), LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity);
                        Debug.LogFormat("GameManager - Scene manager: {0}; Ball created:{1}", LevelManager.Instance, Ball.Instance);
                    }

                    // Spawn the AIs
                    int count = PhotonNetwork.CurrentRoom.MaxPlayers - 1;
                    Team playerTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);
                    Team opponentTeam = playerTeam == Team.Blue ? Team.Red : Team.Blue;
                    int spawnPointId = 1;
                    for(int i=0; i<count; i++)
                    {
                        //PhotonNetwork.CurrentRoom.AddPlayer();
                        Player newPlayer = Player.CreateOfflinePlayer(i + 2);
                        PhotonNetwork.CurrentRoom.AddPlayer(newPlayer);
                        if(i+1 < PhotonNetwork.CurrentRoom.MaxPlayers / 2)
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

                        GameObject newPlayerObject = PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Characters, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                        //GameObject newPlayerObject = GameObject.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                        newPlayerObject.GetComponent<PlayerAI>().Activate();
                        newPlayerObject.GetComponent<PlayerAI>().Team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(newPlayer, PlayerCustomPropertyKey.TeamColor);
                        //newPlayerObject.GetComponent<PlayerAI>().ActorId = i + 2;
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
                if(PlayerController.LocalPlayer)
                    Destroy(PlayerController.LocalPlayer);

            }
            Debug.LogFormat("GameManager - scene loaded [Name:{0}]; inGame:{1}", scene.name, inGame);
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            //Debug.LogFormat("GameManager - OnRoomPropertiesUpdate");
            //foreach(string key in propertiesThatChanged.Keys)
            //{
            //    Debug.LogFormat("GameManager - {0}:{1}", key, propertiesThatChanged[key]);
            //}
        }
#endregion
    }

}
