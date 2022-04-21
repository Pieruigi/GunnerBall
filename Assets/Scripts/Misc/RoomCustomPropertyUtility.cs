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

#if AI_SUPPORT
        public static readonly string AICount = "aic"; // byte
        public static readonly string AINextCodeValue = "anc"; // byte
        public static readonly string AITeamKeyPrefix = "ait_"; // byte
        
#endif
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
            //ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
            //ht.Add(key, value);
            //room.SetCustomProperties(ht);

        }

       

        public static object GetRoomCustomProperty(Room room, string key)
        {
            //Debug.Log("RoomCustomProperties.length:" + room.CustomProperties.Count);
            //Debug.LogFormat("RoomCustomProperties - {0}:{1}",key, room.CustomProperties[key]);
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

#if AI_SUPPORT
        public static void SetAICountCustomProperty(Room room, byte value)
        {
            if (room.CustomProperties.ContainsKey(RoomCustomPropertyKey.AICount))
                room.CustomProperties[RoomCustomPropertyKey.AICount] = value;
            else
                room.CustomProperties.Add(RoomCustomPropertyKey.AICount, value);
        }

        public static void SetAICountCustomProperty(byte value)
        {
            SetAICountCustomProperty(PhotonNetwork.CurrentRoom, value);
        }

        public static bool TryGetAICountCustomProperty(Room room, out byte value)
        {
            value = 0;
            if (!room.CustomProperties.ContainsKey(RoomCustomPropertyKey.AICount))
                return false;

            value = (byte) room.CustomProperties[RoomCustomPropertyKey.AICount];
            return true;
        }

        public static bool TryGetAICountCustomProperty(out byte value)
        {
            return TryGetAICountCustomProperty(PhotonNetwork.CurrentRoom, out value);
        }

        public static void SetAITeamCustomProperty(Room room, byte aiCode, byte value)
        {
            string key = RoomCustomPropertyKey.AITeamKeyPrefix + aiCode.ToString();
            if (room.CustomProperties.ContainsKey(key))
                room.CustomProperties[key] = value;
            else
                room.CustomProperties.Add(key, value);
        }

        public static void SetAITeamCustomProperty(byte aiCode, byte value)
        {
            SetAITeamCustomProperty(PhotonNetwork.CurrentRoom, aiCode, value);
        }

        public static void ResetAITeamCustomProperty(Room room, byte aiCode)
        {
            string key = RoomCustomPropertyKey.AITeamKeyPrefix + aiCode;
            if (!room.CustomProperties.ContainsKey(key))
                return;

            room.CustomProperties[key] = null;
        }

        public static void ResetAITeamCustomProperty(byte aiCode)
        {
            ResetAITeamCustomProperty(PhotonNetwork.CurrentRoom, aiCode);
        }

        public static bool TryGetAITeamCustomProperty(Room room, byte aiCode, out byte value)
        {
            value = 0;
            string key = RoomCustomPropertyKey.AITeamKeyPrefix + aiCode.ToString();
            if (!room.CustomProperties.ContainsKey(key))
                return false;

            value = (byte)room.CustomProperties[key];
            return true;
        }

        public static bool TryGetAITeamCustomProperty(byte aiCode, out byte value)
        {
            return TryGetAITeamCustomProperty(PhotonNetwork.CurrentRoom, aiCode, out value);
        }



        public static void SetAINextCodeValueCustomProperty(Room room, byte value)
        {
            if (room.CustomProperties.ContainsKey(RoomCustomPropertyKey.AINextCodeValue))
                room.CustomProperties[RoomCustomPropertyKey.AINextCodeValue] = value;
            else
                room.CustomProperties.Add(RoomCustomPropertyKey.AINextCodeValue, value);
        }

        public static void SetAINextCodeValueCustomProperty(byte value)
        {
            SetAINextCodeValueCustomProperty(PhotonNetwork.CurrentRoom, value);
        }

        public static bool TryGetAINextCodeCustomProperty(Room room, out byte value)
        {
            value = 0;
            if (!room.CustomProperties.ContainsKey(RoomCustomPropertyKey.AINextCodeValue))
                return false;

            value = (byte)room.CustomProperties[RoomCustomPropertyKey.AINextCodeValue];
            return true;
        }

        public static bool TryGetAINextCodeCustomProperty(out byte value)
        {
            return TryGetAINextCodeCustomProperty(PhotonNetwork.CurrentRoom, out value);
        }

#endif
    }

}
