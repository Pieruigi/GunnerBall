#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
using Photon.Pun;
#endif

namespace Zoca
{
    public class AccountManager : MonoBehaviour
    {
        #region properties
        public static AccountManager Instance { get; private set; }

        public string PlayerName
        {
            get { return playerName; }
        }
        #endregion



        #region private fields
        string playerName = null;
        #endregion

        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

#if !DISABLESTEAMWORKS
                if (SteamManager.Initialized)
                {
                    playerName = SteamFriends.GetPersonaName();
                    //SteamFriends.GetLargeFriendAvatar(SteamFriends.ge)
                    PhotonNetwork.NickName = playerName;

                }
                else
                {
                    PhotonNetwork.NickName = "Unknown";
                }
#endif

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

        #endregion

        #region public methods
        
        #endregion
    }

}

