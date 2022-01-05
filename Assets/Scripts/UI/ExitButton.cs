using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    
    public class ExitButton : MonoBehaviour
    {
        bool leaving;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        /// <summary>
        /// No confirmation required
        /// </summary>
        public void ForceExit()
        {
            OnLeaveGameYes();
        }

        /// <summary>
        /// Asks the player to confirm first
        /// </summary>
        public void ExitWithConfirmation()
        {
            if (leaving)
                return;

            leaving = true;

            
            string message;
            // Ask the player first
            if(GameManager.Instance.InGame)
            {
                message = "Quit match?";
            }
            else
            {
                message = "Quit game?";
            }
            MessageBox.Show(MessageBox.Type.YesNo, message, OnLeaveGameYes, OnLeaveGameNo);

            if (GameManager.Instance.InGame)
                GetComponentInParent<GameMenu>().Close();
        }

        #region private
        void OnLeaveGameYes()
        {

            if (GameManager.Instance.InGame)
            {
                // Leave the game
                GameManager.Instance.LeaveRoom();
            }
            else
            {
                Application.Quit();
            }
        }

        void OnLeaveGameNo()
        {
            leaving = false;

        }
        #endregion
    }

}
