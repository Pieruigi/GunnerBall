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
        public static readonly string WeaponId = "wid";
    }

    public class PlayerCustomPropertyUtility
    {
        public static void SynchronizePlayerCustomProperties(Player player)
        {
            player.SetCustomProperties(player.CustomProperties);
        }

        public static void SynchronizeLocalPlayerCustomProperties()
        {
            SynchronizePlayerCustomProperties(PhotonNetwork.LocalPlayer);
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

        public static object GetPlayerCustomProperty(Player player, string key)
        {

            Debug.Log("GetPlayerCustomProperty - player:" + player);
            Debug.Log("GetPlayerCustomProperty - key:" + key);
            Debug.Log("GetPlayerCustomProperty - keyValue:" + player.CustomProperties[key]);
            if (!player.CustomProperties.ContainsKey(key))
                return null;

            return player.CustomProperties[key];
        }

        public static object GetLocalPlayerCustomProperty(string key)
        {
            return GetPlayerCustomProperty(PhotonNetwork.LocalPlayer, key);
        }


    }

}
