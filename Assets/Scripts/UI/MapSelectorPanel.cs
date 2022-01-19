using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zoca.Collections;

namespace Zoca.UI
{
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
        Button prevButton;

        [SerializeField]
        Button nextButton;

        [SerializeField]
        GameObject onlineLobbyPanel;

        [SerializeField]
        GameObject offlineLobbyPanel;

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
            // Load map collection
            maps = new List<Map>(Resources.LoadAll<Map>(Map.CollectionFolder));

            // Show content
        }

        void Back()
        {
            panel.SetActive(false);

            if (online)
                onlineBackPanel.SetActive(true);
            else
                offlineBackPanel.SetActive(true);

        }

        void Prev()
        {
            // Select previous
            selectedMapIndex--;
            if (selectedMapIndex < 0)
                selectedMapIndex = maps.Count - 1;

            // Set image
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;
        }

        void Next()
        {
            // Select previous
            selectedMapIndex++;
            if (selectedMapIndex > maps.Count - 1)
                selectedMapIndex = 0;

            // Set image
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;
        }

        public void CreateRoom()
        {
            EnableButtons(false);

            Launcher.Instance.CreateRoom(maxPlayers, maps[selectedMapIndex].Id);
        }

        void EnableButtons(bool value)
        {
            createButton.interactable = value;
            backButton.interactable = value;
            nextButton.interactable = value;
            prevButton.interactable = value;

        }

        IEnumerator OpenLobbyDelayed()
        {
            yield return new WaitForEndOfFrame();

            // Reset buttons
            EnableButtons(true);

            if (online)
                onlineLobbyPanel.SetActive(true);
            else
                offlineBackPanel.SetActive(true);

            gameObject.SetActive(false);
        }

        #endregion

        #region public methods
        public void Open(bool online, int maxPlayers)
        {
            // Set fields
            this.online = online;
            this.maxPlayers = maxPlayers;

            // Load all maps
            maps = new List<Map>(Resources.LoadAll<Map>(Map.CollectionFolder));

            // Set the first map as the current one
            selectedMapIndex = 0;
            mapImage.sprite = maps[selectedMapIndex].ImageSprite;

            // Activate panel
            panel.SetActive(true);
        }
        #endregion

        #region pun callbacks
        public override void OnCreatedRoom()
        {

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

            // Show some error message
        }

        public override void OnJoinedRoom()
        {
            // We wait until the frame completed in order to have the player custom properties 
            // set up in the game manager.
            if (!PhotonNetwork.OfflineMode)
                StartCoroutine(OpenLobbyDelayed());
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // Reset buttons
            EnableButtons(true);

        }
        #endregion
    }

}
