using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField]
        GameObject endGameUI;

        bool leavingRoom = false;
       
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
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
            ShowCursor(false);

            Match.Instance.OnStateChanged += HandleOnStateChanged;

            PlayerController.Local.OnLeaveRoomRequest += LeaveGame;

            CloseAll();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            if (PlayerController.Local != null)
                PlayerController.Local.OnLeaveRoomRequest -= LeaveGame;
        }

        public void LeaveGame()
        {
            if (leavingRoom)
                return;

            leavingRoom = true;

            // Show cursor 
            ShowCursor(true);

            // Open the message box
            MessageBox.Show(MessageBox.Type.YesNo, "Do you want to leave the game?", OnLeaveGameYes, OnLeaveGameNo);
        }

        #region private
        void HandleOnStateChanged()
        {
            switch (Match.Instance.State)
            {
                case (int)MatchState.Completed:
                    OpenEndGameUI();
                    break;

            }
            
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
            ShowCursor(true);
        }

        void CloseAll()
        {
            endGameUI.SetActive(false);
        }

        void ShowCursor(bool show)
        {
            if (show)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        void OnLeaveGameYes()
        {
           
            // Leave the game
            GameManager.Instance.LeaveRoom();
        }

        void OnLeaveGameNo()
        {
            leavingRoom = false;

            // Hide the cursor and do nothing
            ShowCursor(false);

        }
        #endregion
    }

}
