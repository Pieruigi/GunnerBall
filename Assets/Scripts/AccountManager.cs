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
                    Debug.LogFormat("My name: {0}", name);
                    PhotonNetwork.NickName = name;


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
        public bool TryGetLocalPlayerAvatarAsTexture2D(out Texture2D texture)
        {
            return TryGetPlayerAvatarAsTexture2D(SteamUser.GetSteamID().m_SteamID, out texture);
        }

        public bool TryGetPlayerAvatarAsTexture2D(ulong userId, out Texture2D texture)
        {
            var avatarInt = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());

            texture = null;
            uint ImageWidth;
            uint ImageHeight;
            bool bIsValid = SteamUtils.GetImageSize(avatarInt, out ImageWidth, out ImageHeight);

            if (bIsValid)
            {
                byte[] Image = new byte[ImageWidth * ImageHeight * 4];

                bIsValid = SteamUtils.GetImageRGBA(avatarInt, Image, (int)(ImageWidth * ImageHeight * 4));

                // Reverse
                byte[] tmp = new byte[ImageWidth * ImageHeight * 4];
                for(int i=0; i<Image.Length; )
                {
                    tmp[i] = Image[Image.Length - 1 - i - 3];
                    tmp[i+1] = Image[Image.Length - 1 - i - 2];
                    tmp[i+2] = Image[Image.Length - 1 - i - 1];
                    tmp[i+3] = Image[Image.Length - 1 - i];
                    i += 4;
                }
            
                if (bIsValid)
                {
                    texture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(Image);
                    texture.Apply();
                    return true;
                }
            }

            return false;
        }
        #endregion
    }

}

