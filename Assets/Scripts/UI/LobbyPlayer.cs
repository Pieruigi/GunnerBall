using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class LobbyPlayer : MonoBehaviour
    {
        public bool IsEmpty
        {
            get { return empty; }
        }

        public bool IsLocked
        {
            get { return locked; }
        }

        public Player Player
        {
            get { return player; }
        }

        [SerializeField]
        Image avatarImage;

        [SerializeField]
        TMP_Text nickText;

        [SerializeField]
        Sprite emptySprite;

        [SerializeField]
        Sprite lockedSprite;

        [SerializeField]
        Sprite aiSprite;

        bool empty = false;
        bool locked = false;
        Player player;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

     

        Texture2D GetPlayerAvatarTexture(Player player)
        {
#if !DISABLESTEAMWORKS
            Debug.Log("Getting user id...");
            long userId = (long)PlayerCustomPropertyUtility.GetPlayerCustomProperty(player, PlayerCustomPropertyKey.UserId);
            Debug.Log("User id: " + userId);
            Texture2D avatar;
            if (SteamUtility.TryGetPlayerAvatarAsTexture2D((ulong)userId, out avatar))
            {
                return avatar;
            }
            return null;
#else 
            return null;
#endif
        }

        public void Init(Player player)
        {
            Debug.Log("Player init...");
            empty = false;
            locked = false;
            this.player = player;

            if(!PhotonNetwork.OfflineMode || player == PhotonNetwork.LocalPlayer)
            {
                Texture2D tex = GetPlayerAvatarTexture(player);
                //Debug.Log("tex.w:" + tex.width);
                //Debug.Log("tex.h:" + tex.height);
                //Debug.Log("avatarImage:" + avatarImage.sprite);
                if (tex != null)
                    avatarImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                else
                    avatarImage.sprite = emptySprite;
                
                nickText.text = player.NickName;
                
            }
            else
            {
                avatarImage.sprite = aiSprite;
                nickText.text = "CPU";
            }
            
        }

        

        public void Clear()
        {
            empty = true;
            locked = false;
            player = null;
            avatarImage.sprite = emptySprite;
            nickText.text = "";
            
        }

        public void Lock()
        {
            empty = true;
            locked = true;
            player = null;
            avatarImage.sprite = lockedSprite;
            nickText.text = "";
            
        }
    }

}
