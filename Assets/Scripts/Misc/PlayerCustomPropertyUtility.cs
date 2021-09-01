using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
   
    public class PlayerCustomPropertyKey
    {
        public static readonly string TeamColor = "tc";
        public static readonly string CharacterId = "cid";
    }

    public class PlayerCustomPropertyUtility
    {
        public static void SynchronizePlayerCustomProperties(Player player)
        {
            player.SetCustomProperties(player.CustomProperties);
        }


        public static void AddOrUpdatePlayerCustomProperty(Player player, string key, object value)
        {
            if (player.CustomProperties.ContainsKey(key))
                player.CustomProperties[key] = value;
            else
                player.CustomProperties.Add(key, value);
        }

        public static void AddOrUpdateLocalPlayerCustomProperty(string key, object value)
        {
            AddOrUpdatePlayerCustomProperty(PhotonNetwork.LocalPlayer, key, value);
        }

        public static bool TryGetPlayerCustomProperty<T>(Player player, string key, ref T value)
        {
            if (!player.CustomProperties.ContainsKey(key))
                return false;

            value = (T)player.CustomProperties[key];
            return true;
        }

        public static bool TryGetLocalPlayerCustomProperty<T>(string key, ref T value)
        {
            return TryGetPlayerCustomProperty<T>(PhotonNetwork.LocalPlayer, key, ref value);
        }
    }

}
