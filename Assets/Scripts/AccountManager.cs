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
        // Start is called before the first frame update
        void Start()
        {
#if !DISABLESTEAMWORKS
            if (SteamManager.Initialized)
            {
                string name = SteamFriends.GetPersonaName();
                Debug.LogFormat("My name: {0}", name);
                PhotonNetwork.NickName = name;
            }
            else
            {
                PhotonNetwork.NickName = "Unknown";
            }
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

