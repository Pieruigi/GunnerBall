//#define TEST_SINGLE_PLAYER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Zoca
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }

        bool inGame = false;

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

        public void Resume()
        {
            
        }

        #region private
        /// <summary>
        /// Only used by the master client
        /// </summary>
        void LoadArena()
        {
            PhotonNetwork.LoadLevel("Arena1vs1");
        }

        #endregion

        #region callbacks



        /// <summary>
        /// This is called whene another player enters the room; 
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
                    {
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

                        //PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                        // The room is full, load the arena
                        LoadArena();

                        
                    }

                }
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("PUN - Player [ID:{0}] left room [Name:{1}].", otherPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
        }

#if TEST_SINGLE_PLAYER && UNITY_EDITOR
        public override void OnJoinedRoom()
        {
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
            // The room is full, load the arena
            LoadArena();
        }
#endif
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
                    
                    if (!PlayerController.LocalPlayer) // Local player is null
                    {
                        int cId = 0;
                        if (!PlayerCustomPropertyUtility.TryGetPlayerCustomProperty<int>(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, ref cId))
                        {
                            Debug.LogErrorFormat("GameManager - Empty property for local player: [{0}]", PlayerCustomPropertyKey.CharacterId);
                        }
                        Debug.LogFormat("Loading local player character [CharacterId:{0}].", cId);
                        GameObject playerPrefab = Resources.LoadAll<PlayerController>(ResourceFolder.Character)[(int)cId].gameObject;
                        Debug.LogFormat("Character found: {0}", playerPrefab.name);
                        Debug.LogFormat("Spawning {0} on photon network...", playerPrefab.name);

                        // Spawn the networked local player ( only support 1vs1 at the moment )
                        // Get the player team
                        Team team = Team.Blue;
                        if (!PlayerCustomPropertyUtility.TryGetPlayerCustomProperty<Team>(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, ref team))
                        {
                            Debug.LogErrorFormat("PlayerController - property is empty: {0}", PlayerCustomPropertyKey.TeamColor);
                        }

                        // Get the spawn point depending on the team the player belongs to
                        Transform spawnPoint = null;
                        if (team == Team.Blue)
                        {
                            spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
                        }
                        else
                        {
                            spawnPoint = LevelManager.Instance.RedTeamSpawnPoints[0];
                        }
                        // Spawn
                        PhotonNetwork.Instantiate(System.IO.Path.Combine(ResourceFolder.Character, playerPrefab.name), spawnPoint.position, spawnPoint.rotation);
                    }

                    
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        // The master client also needs to instantiate the networked ball
                        if (!Ball.Instance)
                        {
                            // For now we only have one ball ( id=0 ) in resources
                            GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Ball)[0].gameObject;
                            PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(ResourceFolder.Ball, ballPrefab.name), LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity); ;
                            Debug.LogFormat("GameManager - Scene manager: {0}; Ball created:{1}", LevelManager.Instance, Ball.Instance);
                        }
                        
                        
                    }



                }
                else // Offline mode: for test, means we are testing the arena from the editor
                {
                    // Hide cursor
                    Cursor.lockState = CursorLockMode.Locked;

                    // Add default custom properties
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.TeamColor, Team.Blue);
                    PlayerCustomPropertyUtility.AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, PlayerCustomPropertyKey.CharacterId, 0);
                    // Get character resource
                    GameObject player = Resources.LoadAll<PlayerController>(ResourceFolder.Character)[0].gameObject;
                    Debug.LogFormat("Character found: {0}", player.name);
                    Debug.LogFormat("Spawning {0} on photon network...", player.name);
                    // Create local player
                    Transform spawnPoint = LevelManager.Instance.BlueTeamSpawnPoints[0];
                    Instantiate(player, spawnPoint.position, spawnPoint.rotation);

                    // Adding local ball
                    GameObject ballPrefab = Resources.LoadAll<Ball>(ResourceFolder.Ball)[0].gameObject;
                    Instantiate(ballPrefab, LevelManager.Instance.BallSpawnPoint.position, Quaternion.identity); ;
                    Debug.LogFormat("GameManager - Scene manager: {0}", LevelManager.Instance);
                }


                
            }
            else
            {
                inGame = false;
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
