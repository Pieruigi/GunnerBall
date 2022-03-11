using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca.UI
{
    public class ModeStatsDetail : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        TMP_Text modeText;

        [SerializeField]
        TMP_Text winText;

        [SerializeField]
        TMP_Text drawText;

        [SerializeField]
        TMP_Text loseText;

        [SerializeField]
        TMP_Text pointsText;

        [SerializeField]
        int numberOfPlayers;

        bool started = false;
        string modeStringFormat = "{0}VS{0}";
        SteamLeaderboard_t leaderboard;
        PlayerStatsPanel playerStatsPanel;

        // Steam callbacks
        CallResult<LeaderboardFindResult_t> leaderboardFindResultCallResult;
        CallResult<LeaderboardScoresDownloaded_t> leaderboardScoresDownloadedCallResult;
        #endregion

        #region private methods
        private void Awake()
        {
            playerStatsPanel = GetComponentInParent<PlayerStatsPanel>();

            modeText.text = string.Format(modeStringFormat, numberOfPlayers / 2);
        }
        // Start is called before the first frame update
        void Start()
        {
            started = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            Debug.Log("Started:"+started);
            
            if (!started)
            {
                started = true;
                return;
            }
                
            
            RequestLeaderboard(StatsManager.Instance.BuildStatName(StatsManager.RankingLeadNamePrefix, numberOfPlayers), OnLeaderboardFindResult);

            // Set stats
            int tmp = 0;
            if (StatsManager.Instance.TryGetStat(StatsManager.Instance.BuildStatName(StatsManager.WonGamesStatNamePrefix, numberOfPlayers), out tmp))
            {
                winText.text = tmp.ToString();
            }
            if (StatsManager.Instance.TryGetStat(StatsManager.Instance.BuildStatName(StatsManager.DrawnGamesStatNamePrefix, numberOfPlayers), out tmp))
            {
                drawText.text = tmp.ToString();
            }
            if (StatsManager.Instance.TryGetStat(StatsManager.Instance.BuildStatName(StatsManager.LostGamesStatNamePrefix, numberOfPlayers), out tmp))
            {
                loseText.text = tmp.ToString();
            }
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
            if (!started)
                return;

            leaderboardFindResultCallResult?.Cancel();
            leaderboardFindResultCallResult?.Dispose();
            leaderboardScoresDownloadedCallResult?.Cancel();
            leaderboardScoresDownloadedCallResult?.Dispose();

            winText.text = "-";
            drawText.text = "-";
            loseText.text = "-";
        }

        /// <summary>
        /// Request leaderboard
        /// </summary>
        /// <param name="leaderboardName"></param>
        /// <param name="apiDispatchDelegate"></param>
        void RequestLeaderboard(string leaderboardName, CallResult<LeaderboardFindResult_t>.APIDispatchDelegate apiDispatchDelegate)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("SteamManager not initialized.");
                return;
            }

            leaderboardFindResultCallResult = CallResult<LeaderboardFindResult_t>.Create(apiDispatchDelegate);
            leaderboardFindResultCallResult.Set(SteamUserStats.FindLeaderboard(leaderboardName));
            
        }

        #endregion


        #region steam callbacks
        void OnLeaderboardFindResult(LeaderboardFindResult_t callback, bool ioFailed)
        {
            if (ioFailed)
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
                users.Add(playerStatsPanel.UserId);
                //DownloadLeaderboardEntriesForUsers(callback.m_hSteamLeaderboard, users, OnLeaderboardScoresDownloaded);
                leaderboardScoresDownloadedCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
                leaderboardScoresDownloadedCallResult.Set(SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboard, users.ToArray(), users.Count));
            }

        }

        void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t callback, bool ioFailed)
        {
            if (ioFailed)
            {
                Debug.Log("Leaderboard - Download entries for users: ioFailed");
                return;
            }

            int score = 0;

            if (callback.m_cEntryCount > 0)
            {
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

            pointsText.text = score.ToString();
            

        }

        #endregion

    }

}
