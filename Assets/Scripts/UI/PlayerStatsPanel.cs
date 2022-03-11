using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Zoca.UI
{
    public class PlayerStatsPanel : MonoBehaviour
    {
        #region properties
        public static PlayerStatsPanel Instance { get; private set; }


        #endregion

        #region private fields
        [SerializeField]
        Image avatarImage;

        [SerializeField]
        TMP_Text nicknameText;

        [SerializeField]
        List<Transform> modeList; // Each index hold a specific mode (1vs1, 2vs2, etc. )

        [SerializeField]
        Button buttonClose;

        [SerializeField]
        Transform root;

        CSteamID userId;
        SteamLeaderboard_t[] leaderboards;
        #endregion

        #region private methods

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                buttonClose.onClick.AddListener(()=> { Close(); });

                // Init leaderboard array
                leaderboards = new SteamLeaderboard_t[modeList.Count];
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            root.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

       

        void Close()
        {
            
            // Hide the panel
            root.gameObject.SetActive(false);

        }

        // Request leaderboard 
        void RequestLeaderboard(string leaderboardName, CallResult<LeaderboardFindResult_t>.APIDispatchDelegate apiDispatchDelegate)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("SteamManager not initialized.");
                return;
            }


            CallResult<LeaderboardFindResult_t> result = CallResult<LeaderboardFindResult_t>.Create(apiDispatchDelegate);
            result.Set(SteamUserStats.FindLeaderboard(leaderboardName));
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
                leaderboards[0] = callback.m_hSteamLeaderboard;

                // Leaderboard has been found, check the dictionary
                string name = SteamUserStats.GetLeaderboardName(callback.m_hSteamLeaderboard);
                Debug.Log("LeaderboardName:" + name);


                // Get local player entry
                List<CSteamID> users = new List<CSteamID>();
                users.Add(SteamUser.GetSteamID());
                //DownloadLeaderboardEntriesForUsers(callback.m_hSteamLeaderboard, users, OnLeaderboardScoresDownloaded);
            }

        }
        #endregion

        #region public methods
        public void Open(CSteamID userId)
        {
            if (root.gameObject.activeSelf)
                root.gameObject.SetActive(false);

            // Show 
            root.gameObject.SetActive(true);
            this.userId = userId;

            // Request leaderboard
            RequestLeaderboard(StatsManager.Instance.BuildStatName(StatsManager.RankingLeadNamePrefix, 2), OnLeaderboardFindResult);
            //APIDispatchDelegate a;
            
        }

        public void SetNickname(string nickname)
        {
            nicknameText.text = nickname;
        }

        public void SetAvatar(Sprite avatar)
        {
            avatarImage.sprite = avatar;   
        }
        #endregion

    }

}
