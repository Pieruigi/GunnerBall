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


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("PUN - New player [ID:{0}] entered the room [Name:{1}].", newPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
            // Only the master client can load the arena
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("PUN - IsMasterClient: {0}", PhotonNetwork.IsMasterClient);
                Debug.LogFormat("PUN - Current room max players: {0}", PhotonNetwork.CurrentRoom.MaxPlayers);
                Debug.LogFormat("PUN - Current room current players: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    // The room is full, load the arena
                    LoadArena();
                }
            }
        }

        public override void OnLeftRoom()
        {
            Debug.LogFormat("PUN - Left room.");
            SceneManager.LoadScene("MainScene");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("PUN - Player [ID:{0}] left room [Name:{1}].", otherPlayer.UserId, PhotonNetwork.CurrentRoom.Name);
        }

        void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.name.Equals("MainScene"))
            {
                inGame = true;

                // After the scene has been loaded we can network instantiate the player character
                int cId = (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerCustomProperties.CharacterId];
                Debug.LogFormat("Loading local player character [CharacterId:{0}].", cId);
                GameObject player = Resources.LoadAll<PlayerController>(ResourceFolders.Characters)[0].gameObject;
                Debug.LogFormat("Character found: {0}", player.name);
                Debug.LogFormat("Spawning {0} on photon network...", player.name);
                PhotonNetwork.Instantiate(System.IO.Path.Combine(ResourceFolders.Characters, player.name), Vector3.zero, Quaternion.identity);
            }
            else
            {
                inGame = false;
            }
            Debug.LogFormat("GameManager - scene loaded [Name:{0}]; inGame:{1}", scene.name, inGame);
        }
        #endregion
    }

}
