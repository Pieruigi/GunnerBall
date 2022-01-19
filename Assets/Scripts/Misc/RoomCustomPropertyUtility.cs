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
        public static readonly string PlayerCreator = "pc"; // byte
        public static readonly string MapId = "mid"; // byte

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

      

        public static object GetRoomCustomProperty(Room room, string key)
        {
            Debug.Log("RoomCustomProperties.length:" + room.CustomProperties.Count);
            Debug.LogFormat("RoomCustomProperties - {0}:{1}",key, room.CustomProperties[key]);
            if (!room.CustomProperties.ContainsKey(key))
                return null;

            return room.CustomProperties[key];
        }
        

        
        public static void AddOrUpdateCurrentRoomCustomProperty(string key, object value)
        {
            AddOrUpdateRoomCustomProperty(PhotonNetwork.CurrentRoom, key, value);
        }

        public static object GetCurrentRoomCustomProperty(string key)
        {
            return GetRoomCustomProperty(PhotonNetwork.CurrentRoom, key);
            
        }

        public static void SynchronizeCurrentRoomCustomProperties()
        {
            SynchronizeRoomCustomProperties(PhotonNetwork.CurrentRoom);
            
        }

      
    }

}
