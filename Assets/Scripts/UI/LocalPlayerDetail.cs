using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class LocalPlayerDetail : MonoBehaviour
    {
        [SerializeField]
        TMP_Text playerText;

        [SerializeField]
        Image playerImage;

        [SerializeField]
        Button statsButton;

        private void Awake()
        {
            statsButton.onClick.AddListener(OpenPlayerStats);
        }

        // Start is called before the first frame update
        void Start()
        {
            playerText.text = AccountManager.Instance.PlayerName;

            Texture2D avatar;
            if(SteamUtility.TryGetLocalPlayerAvatarAsTexture2D(out avatar))
            {
                Debug.Log("Texture found");
                playerImage.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), Vector2.zero);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OpenPlayerStats()
        {
            PlayerStatsPanel.Instance.Open(SteamUser.GetSteamID());

            PlayerStatsPanel.Instance.SetNickname(AccountManager.Instance.PlayerName);
            PlayerStatsPanel.Instance.SetAvatar(playerImage.sprite);
        }
    }

}
