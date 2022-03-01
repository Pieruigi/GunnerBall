using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class StatsManager : MonoBehaviour
    {
        #region const
        public const string WinStatName = "S_Win"; // Int
        public const string DrawStatName = "S_Draw"; // Int
        public const string LoseStatName = "S_Lose"; // Int
        #endregion

        #region properties
        public static StatsManager Instance { get; private set; }
        #endregion

        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

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
            RequestCurrentStats();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                int statValue;
                string statName = WinStatName;
                if(GetStat(statName, out statValue))
                {
                    Debug.LogFormat("Stat found - {0}:{1}", statName, statValue);
                }
                else
                {
                    Debug.LogFormat("Stat not found - {0}", statName);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                
                string statName = WinStatName;
                int statValue;
                GetStat(statName, out statValue);
                statValue++;
                if (SetStat(statName, statValue))
                {
                    Debug.LogFormat("Stat updated succeeded: " + statName);
                    Debug.Log("UpdateStats:" + StoreStats());
                }
                else
                {
                    Debug.LogFormat("SetStat failed:" + statName);
                }
            }
        }

        void RequestCurrentStats()
        {
            if (!SteamManager.Initialized)
                return;

            Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            SteamUserStats.RequestCurrentStats();
        }

        #endregion

        #region steam callbacks

        void OnUserStatsReceived(UserStatsReceived_t ret)
        {
            if(ret.m_eResult == EResult.k_EResultFail)
            {
                Debug.Log("UserStatsRequest failed");
            }
            else
            {
                Debug.Log("UserStatsRequest succeeded");
            }
        }
        #endregion

        #region public methods
        public bool GetStat(string statName, out int data)
        {
            return SteamUserStats.GetStat(statName, out data);
        }

        public bool SetStat(string statName, int data)
        {
            
            return SteamUserStats.SetStat(statName, data);
            
        }

        public bool SetStat(string statName, float data)
        {
            return SteamUserStats.SetStat(statName, data);
            
        }

        public bool StoreStats()
        {
            return SteamUserStats.StoreStats();
        }
        #endregion
    }

}
