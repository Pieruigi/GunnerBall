using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class RoomCustomPropertyKey
    {
        public static readonly string MatchLength = "ml";
        public static readonly string StartTime = "st";
        public static readonly string MatchState = "ms";
        public static readonly string MatchElapsed = "me";

        //public static readonly float MatchTimeValueDefault = 300f;
    }

    public class RoomCustomPropertyUtility
    {
        //public static void AddOrUpdateRoomCustomProperty(Room room, string key, object value)
        //{
        //    if(room.CustomProperties.ContainsKey(key))
        //    {
        //        room.CustomProperties[key] = value;
        //    }
        //    else
        //    {
        //        room.CustomProperties.
        //    }
        //}
        public static bool TryGetRoomCustomProperty<T>(Room room, string key, ref T value)
        {
            if (!room.CustomProperties.ContainsKey(key))
                return false;

            value = (T)room.CustomProperties[key];
            return true;
        }


    }

}
