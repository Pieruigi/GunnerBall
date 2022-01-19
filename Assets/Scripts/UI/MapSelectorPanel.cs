using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zoca.Collections;

namespace Zoca.UI
{
    public class MapSelectorPanel : MonoBehaviour
    {
        public UnityAction<int> OnMapSelected;


        [SerializeField]
        GameObject panel;

        [SerializeField]
        Button backButton;

        [SerializeField]
        GameObject onlineBackPanel;

        [SerializeField]
        GameObject offlineBackPanel;

        GameObject backPanel;
        bool online;
        int maxPlayers;
        List<Map> maps;

        private void Awake()
        {
            backButton.onClick.AddListener(Back);
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

        private void OnEnable()
        {
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

        #region public methods
        public void Open(bool online, int maxPlayers)
        {
            this.online = online;
            this.maxPlayers = maxPlayers;
            panel.SetActive(true);
        }
        #endregion
    }

}
