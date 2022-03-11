using Photon.Pun;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Zoca
{
    /// <summary>
    /// This class update current stats and leaderboard at the end of the match.
    /// Local stats are downloaded once at startup and local cached.
    /// For leaderboard instead we need to send a request to steamworks everytime we need to 
    /// update it because it's not cached.
    /// </summary>
    public class StatsManager : MonoBehaviour
    {
        // Test steamId:76561199247920575
        
        #region consts
        // Stats and leaderboard ids
        public const string WonGamesStatNamePrefix = "S_WonGames_";
        public const string DrawnGamesStatNamePrefix = "S_DrawnGames_";
        public const string LostGamesStatNamePrefix = "S_LostGames_";
        public const string QuittedGamesStatNamePrefix = "S_QuittedGames_";
        public const string RankingLeadNamePrefix = "L_Ranking_";

        
        // All the points we gain at the end of the match
        public const int VictoryPoints = 300;
        public const int DrawPoints = 100;
        public const int DefeatPoints = 0;

        #endregion

        #region properties
        public static StatsManager Instance { get; private set; }
        #endregion

        #region private fields
        int leaderboardsPointsToAdd = 0;
        SteamLeaderboard_t leaderboard;
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
            // Stats are stored in local cache so they are always updated since the only one
            // who can modify them is the local client.
            RequestCurrentStats();

            SceneManager.sceneLoaded += OnSceneLoaded;
          
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
            //if (Input.GetKeyDown(KeyCode.D))
            //{

            //    leaderboardsPointsToAdd = 100;
            //    string name = RankingLeadNamePrefix + 2.ToString();

            //    Debug.Log("Call request");

            //    //UpdateLocalPlayerScore(leaderboardLocalScoreDownloaded);               
            //    UpdateLeaderboardScore(name);
            //    int cValue;
            //    if (GetStat(WonGamesStatNamePrefix + 2.ToString(), out cValue))
            //    {
            //        Debug.Log("StatValue:" + cValue);
            //        SetStat(WonGamesStatNamePrefix + 2.ToString(), cValue + 1);
            //    }

            //}


        }

        /// <summary>
        /// This methods load the rigth leaderboard from steamworks and update the local 
        /// player score.
        /// We load leaderboards every time in order to keep everything updated.
        /// </summary>
        /// <param name="leaderboardName"></param>
        void UpdateLeaderboardScore(string leaderboardName)
        {
            RequestLocalLeaderboard(leaderboardName, OnLeaderboardFindResult);
        }

        /// <summary>
        /// Load the local player stats.
        /// This methods is called on start when player launch the game and connect to Steam 
        /// because stats are cached in local ( so we don't need to send a request every time ).
        /// </summary>
        void RequestCurrentStats()
        {
            if (!SteamManager.Initialized)
                return;

            Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            SteamUserStats.RequestCurrentStats();
        }

        /// <summary>
        /// Internal only.
        /// Called when local client updates player leaderboard.
        /// </summary>
        /// <param name="leaderboardName"></param>
        /// <param name="apiDispatchDelegate"></param>
        void RequestLocalLeaderboard(string leaderboardName, CallResult<LeaderboardFindResult_t>.APIDispatchDelegate apiDispatchDelegate)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("SteamManager not initialized.");
                return;
            }

            
            CallResult<LeaderboardFindResult_t> result = CallResult<LeaderboardFindResult_t>.Create(apiDispatchDelegate);
            result.Set(SteamUserStats.FindLeaderboard(leaderboardName));
        }

        /// <summary>
        /// Internal only.
        /// Called when local client updates player leaderboard.
        /// </summary>
        /// <param name="leaderboard"></param>
        /// <param name="users"></param>
        /// <param name="apiDispatchDelegate"></param>
        void DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t leaderboard, List<CSteamID> users, CallResult<LeaderboardScoresDownloaded_t>.APIDispatchDelegate apiDispatchDelegate)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("SteamManager not initialized.");
                return;
            }
            

            if(leaderboard == null)
            {
                Debug.LogWarning("Leaderboard handle is null.");
                return;
            }    
            
            
            CallResult<LeaderboardScoresDownloaded_t> result = CallResult<LeaderboardScoresDownloaded_t>.Create(apiDispatchDelegate);
            result.Set(SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboard, users.ToArray(), users.Count));
        }

        /// <summary>
        /// Set user stat
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool SetStat(string statName, int data)
        {

            return SteamUserStats.SetStat(statName, data);

        }

        /// <summary>
        /// Set user stat.
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool SetStat(string statName, float data)
        {
            return SteamUserStats.SetStat(statName, data);

        }

        /// <summary>
        /// Store stats on steam server.
        /// </summary>
        /// <returns></returns>
        bool StoreStats()
        {
            return SteamUserStats.StoreStats();
        }
        #endregion

        #region callbacks
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (GameManager.Instance.InGame)
            {
                // Set the match handles
                Match.Instance.OnStateChanged += OnMatchStateChanged;
            }
        }

        void OnMatchStateChanged()
        {
            Debug.Log("Stats state changed");

            // Only update stats in online matches
            if (PhotonNetwork.OfflineMode)
                return;

            switch (Match.Instance.State)
            {
                case (int)MatchState.Completed:

                    Debug.Log("Stats before the update:");
                    DebugStats();
                    // Get the local player team
                    Team localTeam = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);

                    // Get team scores
                    int blueTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore);
                    int redTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore);

                    Debug.Log("Teams got.");

                    int cValue;
                    if (blueTeamScore == redTeamScore)
                    {
                        // You draw
                        leaderboardsPointsToAdd = DrawPoints;
                        UpdateLeaderboardScore(BuildStatName(RankingLeadNamePrefix));

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
                            leaderboardsPointsToAdd = VictoryPoints;
                            UpdateLeaderboardScore(BuildStatName(RankingLeadNamePrefix));

                            if (GetStat(BuildStatName(WonGamesStatNamePrefix), out cValue))
                                SetStat(BuildStatName(WonGamesStatNamePrefix), cValue + 1);
                        }
                        else
                        {
                            // You lose
                            leaderboardsPointsToAdd = DefeatPoints;
                            if (leaderboardsPointsToAdd != 0)
                                UpdateLeaderboardScore(BuildStatName(RankingLeadNamePrefix));

                            if (GetStat(BuildStatName(LostGamesStatNamePrefix), out cValue))
                                SetStat(BuildStatName(LostGamesStatNamePrefix), cValue + 1);
                        }
                    }
                    Debug.Log("Stats after the update:");
                    DebugStats();
                    break;
            }
        }

        #endregion

    

        #region steam callbacks
        /// <summary>
        /// Internal only.
        /// Called on local leaderboard update.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="ioFailed"></param>
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
                leaderboard = callback.m_hSteamLeaderboard;

                // Leaderboard has been found, check the dictionary
                string name = SteamUserStats.GetLeaderboardName(callback.m_hSteamLeaderboard);
                Debug.Log("LeaderboardName:" + name);

                // Get local player entry
                List<CSteamID> users = new List<CSteamID>();
                users.Add(SteamUser.GetSteamID());
                DownloadLeaderboardEntriesForUsers(callback.m_hSteamLeaderboard, users, OnLeaderboardScoresDownloaded);
            }
            
        }

        /// <summary>
        /// Internal only.
        /// Called on local leaderboard update.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="ioFailed"></param>
        void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t callback, bool ioFailed)
        {
            if (ioFailed)
            {
                Debug.Log("Leaderboard - Download entries for users: ioFailed");
                return;
            }

            int score = 0;

            ELeaderboardUploadScoreMethod uploadMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate;
            if (callback.m_cEntryCount > 0)
            {
                uploadMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;
                Debug.Log("Entry found");
                LeaderboardEntry_t entry;
                if (!SteamUserStats.GetDownloadedLeaderboardEntry(callback.m_hSteamLeaderboardEntries, 0, out entry, null, 0))
                {
                    Debug.LogWarning("Can't get entry.");
                    return;
                }
                score = entry.m_nScore;
                Debug.Log("Entryscore:" + score);
            }

            score += leaderboardsPointsToAdd;
            Debug.Log("New score:" + score);
            CallResult<LeaderboardScoreUploaded_t> result = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUpdloaded);
            result.Set(SteamUserStats.UploadLeaderboardScore(leaderboard, uploadMethod, score, null, 0));

            Debug.Log("Leaderboard - Upload call sent.");
           
        }

        /// <summary>
        /// Internal only.
        /// Called on local leaderboard update.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="ioFailure"></param>

        void OnLeaderboardScoreUpdloaded(LeaderboardScoreUploaded_t callback, bool ioFailure)
        {
            if (ioFailure)
            {
                Debug.LogWarning("Leaderboard score upload failed: IO");
                return;
            }

            Debug.Log("Leaderboard score upload result:" + (callback.m_bSuccess == 1));
            
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



        public string BuildStatName(string statNamePrefix)
        {
            //return statNamePrefix + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            return BuildStatName(statNamePrefix, PhotonNetwork.CurrentRoom.MaxPlayers);
        }

        public string BuildStatName(string statNamePrefix, int numberOfPlayers)
        {
            return statNamePrefix + numberOfPlayers.ToString();
        }
        #endregion

        #region debug
        void DebugStats()
        {
           
            int cValue;
            GetStat(BuildStatName(WonGamesStatNamePrefix), out cValue);
            Debug.LogFormat("Wins:" + cValue);
            GetStat(BuildStatName(DrawnGamesStatNamePrefix), out cValue);
            Debug.LogFormat("Drawn:" + cValue);
            GetStat(BuildStatName(LostGamesStatNamePrefix), out cValue);
            Debug.LogFormat("Lost:" + cValue);
        }
        #endregion
    }

}
