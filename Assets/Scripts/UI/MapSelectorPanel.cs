using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zoca.Collections;

namespace Zoca.UI
{
    /// <summary>
    /// This panel is open as a popup over the online panel in order to let the player who
    /// is creating a new room to choose the map.
    /// NB: the online panel is not going to be closed so it still receives rooms update until
    /// you press the create button.
    /// </summary>
    public class MapSelectorPanel : MonoBehaviourPunCallbacks
    {
        #region actions
        public UnityAction<int> OnMapSelected;

        #endregion

        #region private fields

        [SerializeField]
        GameObject panel;

         

        [SerializeField]
        Button backButton;

        [SerializeField]
        Button createButton;

        [SerializeField]
        GameObject onlineBackPanel;

        [SerializeField]
        GameObject offlineBackPanel;

        [SerializeField]
        Image mapImage;

        [SerializeField]
        TMP_Text mapNameText;

        [SerializeField]
        Button prevButton;

        [SerializeField]
        Button nextButton;

        [SerializeField]
        GameObject onlineLobbyPanel;

        [SerializeField]
        GameObject offlineLobbyPanel;

        [SerializeField]
        GameObject comingSoonObject;

        GameObject backPanel;
        bool online;
        int maxPlayers;
        List<Map> maps;
        int selectedMapIndex;
        #endregion

        #region private methods

        private void Awake()
        {
           
            // Add button actions
            backButton.onClick.AddListener(Back);
            prevButton.onClick.AddListener(Prev);
            nextButton.onClick.AddListener(Next);
            createButton.onClick.AddListener(CreateRoom);
        }

        // Start is called before the first frame update
        void Start()
        {
            
            panel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
            // Get available maps for this player
            maps = new List<Map>(MapManager.Instance.GetAvailableMaps());
            // Show content
        }

        void Back()
        {
            panel.SetActive(false);

            if (online)
                onlineBackPanel.SetActive(false);
            else
            {
                offlineBackPanel.SetActive(false);
                
            }
                

        }

        void Prev()
        {
            //StartCoroutine(ShowComingSoonText());
            //return;

            // Select previous
            selectedMapIndex--;
            if (selectedMapIndex < 0)
                selectedMapIndex = maps.Count - 1;

            // Set image
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;
            // Set name
            mapNameText.text = maps[selectedMapIndex].Name;
        }

        void Next()
        {
            //StartCoroutine(ShowComingSoonText());
            //return;

            // Select previous
            selectedMapIndex++;
            if (selectedMapIndex > maps.Count - 1)
                selectedMapIndex = 0;

            // Set image
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;
            // Set name
            mapNameText.text = maps[selectedMapIndex].Name;
        }

        IEnumerator ShowComingSoonText()
        {
            comingSoonObject.SetActive(true);
            yield return new WaitForSeconds(3);

            comingSoonObject.SetActive(false);
        }

        public void CreateRoom()
        {
            EnableButtons(false);

            //Launcher.Instance.CreateRoom(maxPlayers, maps[selectedMapIndex].Id);
            if(online)
                transform.root.GetComponentInChildren<LauncherPanel>().CreateRoom(maxPlayers, maps[selectedMapIndex].Id);
            else
                transform.root.GetComponentInChildren<LauncherPanel>().CreateRoom(maxPlayers, maps[selectedMapIndex].Id);
            
        }

        void EnableButtons(bool value)
        {
            createButton.interactable = value;
            backButton.interactable = value;
            nextButton.interactable = value;
            prevButton.interactable = value;

        }

        #endregion

        #region public methods
        public void OpenOffline()
        {
            Open(false, 2);
        }

        public void Open(bool online, int maxPlayers)
        {
            // Set fields
            this.online = online;
            this.maxPlayers = maxPlayers;

            // Load all maps
            // Get all the available maps for this player
            maps = new List<Map>(MapManager.Instance.GetAvailableMaps());

            // Set the first map as the current one
            selectedMapIndex = 0;
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;
            // Set name
            mapNameText.text = maps[selectedMapIndex].Name;

            comingSoonObject.SetActive(false);

            // Activate panel
            panel.SetActive(true);
        }
        #endregion

        #region pun callbacks
        public override void OnCreatedRoom()
        {
            Debug.Log("Room Created...");
            // Reset buttons
            EnableButtons(true);

            // Close
            gameObject.SetActive(false);


        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room Creation failed...");

            // Reset buttons
            EnableButtons(true);

            // Close
            gameObject.SetActive(false);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

        }
        #endregion
    }

}
