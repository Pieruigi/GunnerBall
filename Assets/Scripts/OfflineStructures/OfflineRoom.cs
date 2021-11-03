using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    /// <summary>
    /// A dedicated structure to manage offline room data, such as custom properties and players.
    /// It's been implemented having compatibility in mind.
    /// </summary>
    public class OfflineRoom
    {
        // Internal use
        //static OfflineRoom instance;

        static Dictionary<string, object> customProperties;
        public static Dictionary<string, object> CustomProperties
        {
            get { return customProperties; }
        }

        int playerMax;

        OfflinePlayer[] players;

        private OfflineRoom(int playerMax) 
        {
            customProperties = new Dictionary<string, object>();
            this.playerMax = playerMax;
            players = new OfflinePlayer[playerMax];
        }

        /// <summary>
        /// This method is called every time a new offline match starts
        /// </summary>
        public static void Create(int playerMax)
        {
            OfflineRoom room = new OfflineRoom(playerMax);
            
        }

        public static void AddOrUpdateCustomProperty(string key, object value)
        {


            if (!customProperties.ContainsKey(key))
                customProperties.Add(key, value);
            else
                customProperties[key] = value;
        }

        public static object GetCustomProperty(string key)
        {
            return customProperties[key];
        }
    }

}
