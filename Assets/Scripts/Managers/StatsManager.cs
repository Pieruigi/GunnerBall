using Photon.Pun;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class StatsManager : MonoBehaviour
    {
        // Test steamId:76561199247920575
        
        #region consts
        public const string WonGamesStatNamePrefix = "S_WonGames_";
        public const string DrawnGamesStatNamePrefix = "S_DrawnGames_";
        public const string LostGamesStatNamePrefix = "S_LostGames_";
        public const string QuittedGamesStatNamePrefix = "S_QuittedGames_";

        public const string RankingLeadNamePrefix = "L_Ranking_";

        public const int VictoryPoints = 300;
        public const int DrawPoints = 100;
        public const int DefeatPoints = 0;


        #endregion

        #region properties
        public static StatsManager Instance { get; private set; }
        #endregion

        #region private fields
        Dictionary<string, SteamLeaderboard_t> leaderboards = new Dictionary<string, SteamLeaderboard_t>();
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
            // Request all the local user stats
            RequestCurrentStats();

            RequestCurrentLeaderboards();

            if (GameManager.Instance.InGame)
            {
                // Set the match handles
                Match.Instance.OnStateChanged += OnMatchStateChanged;
            }
          
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    int statValue;
            //    string statName = WonGamesStatNamePrefix + 2.ToString();
            //    if (GetStat(statName, out statValue))
            //    {
            //        Debug.LogFormat("Stat found - {0}:{1}", statName, statValue);
            //    }
            //    else
            //    {
            //        Debug.LogFormat("Stat not found - {0}", statName);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{

            //    string statName = WonGamesStatNamePrefix + 2.ToString();
            //    int statValue;
            //    GetStat(statName, out statValue);
            //    statValue++;
            //    if (SetStat(statName, statValue))
            //    {
            //        Debug.LogFormat("Stat updated succeeded: " + statName);
            //        Debug.Log("UpdateStats:" + StoreStats());
            //    }
            //    else
            //    {
            //        Debug.LogFormat("SetStat failed:" + statName);
            //    }
            //}

            
           
        }

        void RequestCurrentStats()
        {
            if (!SteamManager.Initialized)
                return;

            Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            SteamUserStats.RequestCurrentStats();
        }

        void RequestCurrentLeaderboards()
        {
            //SteamGameServerStats.SetUserStat();
            if (!SteamManager.Initialized)
                return;
            for(int i=1; i<=5; i++)
            {
                CallResult<LeaderboardFindResult_t> result = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
                result.Set(SteamUserStats.FindLeaderboard(RankingLeadNamePrefix + (2 * i).ToString()));
            }
            
        }
        #endregion

        #region callbacks
        void OnMatchStateChanged()
        {
            // Only update stats in online matches
            if (PhotonNetwork.OfflineMode)
                return;

            switch (Match.Instance.State)
            {
                case (int)MatchState.Completed:

                    // Get the local player team
                    Team localTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);

                    // Get team scores
                    int blueTeamScore = (int)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore);
                    int redTeamScore = (int)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore);

                    int cValue;
                    if (blueTeamScore == redTeamScore)
                    {
                        // You draw
                        if (GetStat(BuildStatName(DrawnGamesStatNamePrefix), out cValue))
                            SetStat(BuildStatName(DrawnGamesStatNamePrefix), cValue + 1);
                    }
                    else
                    {
                        // You win or lose
                        if ((localTeam == Team.Blue && blueTeamScore > redTeamScore) ||
                            (localTeam == Team.Red && blueTeamScore < redTeamScore))
                        {
                            // You win
                            if (GetStat(BuildStatName(WonGamesStatNamePrefix), out cValue))
                                SetStat(BuildStatName(WonGamesStatNamePrefix), cValue + 1);
                        }
                        else
                        {
                            // You lose
                            if (GetStat(BuildStatName(LostGamesStatNamePrefix), out cValue))
                                SetStat(BuildStatName(LostGamesStatNamePrefix), cValue + 1);
                        }
                    }

                    break;
            }
        }
        #endregion

    

        #region steam callbacks
        void OnLeaderboardFindResult(LeaderboardFindResult_t callback, bool ioFailed)
        {
            if(ioFailed)
            {
                Debug.Log("Leaderboard - ioFailed");
                return;
            }

            Debug.Log("Leaderboard found:" + callback.m_bLeaderboardFound);
            
            if (callback.m_bLeaderboardFound == 1)
            {
                // Leaderboard has been found, check the dictionary
                string name = SteamUserStats.GetLeaderboardName(callback.m_hSteamLeaderboard);
                Debug.Log("LeaderboardName:" + name);
                if (!leaderboards.ContainsKey(name))
                    leaderboards.Add(name, callback.m_hSteamLeaderboard);
                else
                    leaderboards[name] = callback.m_hSteamLeaderboard;
            }
        }

        /// <summary>
        /// Receiving all the user stats
        /// </summary>
        /// <param name="ret"></param>
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
        /// <summary>
        /// Get user stat.
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetStat(string statName, out int data)
        {
            return SteamUserStats.GetStat(statName, out data);
        }

        /// <summary>
        /// Set user stat
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetStat(string statName, int data)
        {
            
            return SteamUserStats.SetStat(statName, data);
            
        }

        /// <summary>
        /// Set user stat.
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetStat(string statName, float data)
        {
            return SteamUserStats.SetStat(statName, data);
            
        }

        /// <summary>
        /// Store stats on steam server.
        /// </summary>
        /// <returns></returns>
        public bool StoreStats()
        {
            return SteamUserStats.StoreStats();
        }

        //public bool SetLeaderboard(string leaderboardName)
        //{
        //    SteamUserStats.
        //}

        public string BuildStatName(string statNamePrefix)
        {
            return statNamePrefix + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        }
        #endregion
    }

}
