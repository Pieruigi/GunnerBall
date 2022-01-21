#if !DISABLESTEAMWORKS
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class SteamUtility
    {
        public static bool TryGetLocalPlayerAvatarAsTexture2D(out Texture2D texture)
        {
            return TryGetPlayerAvatarAsTexture2D(SteamUser.GetSteamID().m_SteamID, out texture);
        }

        public static bool TryGetPlayerAvatarAsTexture2D(ulong userId, out Texture2D texture)
        {
            CSteamID steamId = new CSteamID(userId);

            var avatarInt = SteamFriends.GetMediumFriendAvatar(steamId);

            texture = null;
            uint ImageWidth;
            uint ImageHeight;
            bool bIsValid = SteamUtils.GetImageSize(avatarInt, out ImageWidth, out ImageHeight);

            if (bIsValid)
            {
                byte[] Image = new byte[ImageWidth * ImageHeight * 4];

                bIsValid = SteamUtils.GetImageRGBA(avatarInt, Image, (int)(ImageWidth * ImageHeight * 4));



                if (bIsValid)
                {
                    texture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(Image);
                    texture = GeneralUtility.Texture2DFlipVertical(texture);
                    texture.Apply();
                    return true;
                }
            }

            return false;
        }
    }

}
#endif