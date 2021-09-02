using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class RoomCustomPropertyKey
    {
        public static readonly string MatchLength = "ml"; // int
        public static readonly string MatchStateTimestamp = "mst"; // float
        public static readonly string MatchState = "ms"; // byte
        public static readonly string MatchOldState = "mos"; // byte
        public static readonly string MatchTimeElapsed = "mte"; // float
        public static readonly string BlueTeamScore = "bts"; // byte
        public static readonly string RedTeamScore = "rts"; // byte
        
        //public static readonly float MatchTimeValueDefault = 300f;
    }

    public class RoomCustomPropertyUtility
    {
        public static void SynchronizeRoomCustomProperties(Room room)
        {
            if (room == null)
            {
                Debug.LogWarningFormat("RoomCustomPropertyUtility - No room available, Room:{0}", room);
                return;
            }

            room.SetCustomProperties(room.CustomProperties);
        }

        public static void SynchronizeCurrentRoomCustomProperties()
        {
            SynchronizeRoomCustomProperties(PhotonNetwork.CurrentRoom);
        }

        public static void AddOrUpdateRoomCustomProperty(Room room, string key, object value)
        {
            if(room == null)
            {
                Debug.LogWarningFormat("RoomCustomPropertyUtility - No room available, Room:{0}", room);
                return;
            }

            if (room.CustomProperties.ContainsKey(key))
            {
                room.CustomProperties[key] = value;
            }
            else
            {
                room.CustomProperties.Add(key, value);
            }

           
        }

        public static void AddOrUpdateCurrentRoomCustomProperty(string key, object value)
        {
            AddOrUpdateRoomCustomProperty(PhotonNetwork.CurrentRoom, key, value);
        }

        public static object GetRoomCustomProperty(Room room, string key)
        {
            if (!room.CustomProperties.ContainsKey(key))
                return null;

            return room.CustomProperties[key];
        }

        public static object GetCurrentRoomCustomProperty(string key)
        {
            return GetRoomCustomProperty(PhotonNetwork.CurrentRoom, key);
        }

        public static bool TryGetRoomCustomProperty<T>(Room room, string key, ref T value)
        {
            if (room == null)
            {
                Debug.LogWarningFormat("RoomCustomPropertyUtility - No room available, Room:{0}", room);
                return false;
            }

            if (!room.CustomProperties.ContainsKey(key))
                return false;

            value = (T)room.CustomProperties[key];
            return true;
        }

        public static bool TryGetCurrentRoomCustomProperty<T>(string key, ref T value)
        {
            return TryGetRoomCustomProperty<T>(PhotonNetwork.CurrentRoom, key, ref value);
        }

    }

}
