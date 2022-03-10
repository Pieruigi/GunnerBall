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
        #endregion

        #region private methods

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                buttonClose.onClick.AddListener(()=> { Close(); });
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

        private void OnEnable()
        {
            Debug.Log("OnEnabled");
        }

        private void OnDisable()
        {
            Debug.Log("OnDisabled");
        }

        void Close()
        {
            
            // Hide the panel
            root.gameObject.SetActive(false);
        }
        #endregion

        #region public methods
        public void Open(CSteamID userId)
        {
            if (root.gameObject.activeSelf)
                root.gameObject.SetActive(false);

            // Show 
            root.gameObject.SetActive(true);
           

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
