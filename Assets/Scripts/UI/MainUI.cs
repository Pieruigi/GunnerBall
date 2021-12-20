using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class MainUI : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Button button1vs1;

        [SerializeField]
        Button button2vs2;
        #endregion

        #region private fields
        Launcher launcher;
        #endregion

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
            launcher = FindObjectOfType<Launcher>();

            // Set callbacks
            button1vs1.onClick.AddListener(() => CreateMatch(2));
            button2vs2.onClick.AddListener(() => CreateMatch(4));
        }

        // Update is called once per frame
        void Update()
        {
            // Check match buttons
            //if(PhotonNetwork.LocalPlayer.)
        }

        #endregion

        #region public methods
        public void CreateMatch(int maxPlayers)
        {
            launcher.CreateMatch(maxPlayers);
        }
        #endregion
    }

}
