using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.UI;

namespace Zoca
{
    public class UIManager : MonoBehaviourPunCallbacks
    {
        public static UIManager Instance { get; private set; }

        [SerializeField]
        GameObject endGameUI;

        [SerializeField]
        GameObject opponentLeftUI;

        [SerializeField]
        GameObject gameMenuUI;


        bool leavingRoom = false;
        //GameMenu gameMenu;

        #region private
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                //gameMenu = gameMenuUI.GetComponent<GameMenu>();
                     
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Hide cursor
            GeneralUtility.ShowCursor(false);

            Match.Instance.OnStateChanged += HandleOnStateChanged;

            PlayerController.Local.OnPaused += PauseGame;

            CloseAll();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            if (PlayerController.Local != null)
                PlayerController.Local.OnPaused -= PauseGame;
        }

     

       
        void HandleOnStateChanged()
        {
            switch (Match.Instance.State)
            {
                case (int)MatchState.Completed:
                    OpenEndGameUI();
                    break;

            }
            
        }

        void OpenPlayerLeftUI(string playerName)
        {
            // Does nothing if already hidden
            MessageBox.Hide();

            // Close any ui
            CloseAll();

            // Open end game ui
            //opponentLeftUI.SetActive(true);
            opponentLeftUI.GetComponent<OpponentLeft>().Show(playerName);

            // Show cursor
            GeneralUtility.ShowCursor(true);
        }

        void OpenEndGameUI()
        {
            // Does nothing if already hidden
            MessageBox.Hide();

            // Close any ui
            CloseAll();

            // Open end game ui
            endGameUI.SetActive(true);

            // Show cursor
            GeneralUtility.ShowCursor(true);
        }

        void CloseAll()
        {
            endGameUI.SetActive(false);
            //opponentLeftUI.SetActive(false);
        }

 

        /// <summary>
        /// Called when another player leaves
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("PUN - Player [ID:{0}] left room [Name:{1}].", otherPlayer.UserId, PhotonNetwork.CurrentRoom.Name);

            // If the game started we must tell the other player that the opponent
            // left and the game is closed
            if (GameManager.Instance.InGame)
            {
                OpenPlayerLeftUI(otherPlayer.NickName);
            }
        }

        void PauseGame()
        {
            if (leavingRoom || MessageBox.IsVisible() || endGameUI.activeSelf)// || opponentLeftUI.activeSelf)
                return;
            
            //Debug.Log("Paused game");
            if (!gameMenuUI.activeSelf)
                OpenGameMenuUI();
            else
                CloseGameMenuUI();
        }
        #endregion

        #region public


        public void OpenGameMenuUI()
        {
            

            if (leavingRoom)
                return;
            if (MessageBox.IsVisible())
                return;
            if (gameMenuUI.activeSelf)
                return;


            GeneralUtility.ShowCursor(true);
            gameMenuUI.SetActive(true);
        }

        public void CloseGameMenuUI()
        {
            if (!gameMenuUI.activeSelf)
                return;

            gameMenuUI.SetActive(false);
            GeneralUtility.ShowCursor(false);
        }
        #endregion
    }

}
