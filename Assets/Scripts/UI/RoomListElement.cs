using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca.UI
{
    public class RoomListElement : MonoBehaviour
    {
        public RoomInfo RoomInfo
        {
            get { return roomInfo; }
        }

        [SerializeField]
        TMP_Text roomNameField;

        [SerializeField]
        TMP_Text playerCountField;

        [SerializeField]
        TMP_Text mapNameField;

     
        RoomInfo roomInfo;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            roomNameField.text = (string) roomInfo.CustomProperties[RoomCustomPropertyKey.PlayerCreator];
            mapNameField.text = MapManager.Instance.GetMap((byte)roomInfo.CustomProperties[RoomCustomPropertyKey.MapId]).Name;
            playerCountField.text = string.Format("{0}/{1}", roomInfo.PlayerCount, roomInfo.MaxPlayers);
        }
    }

}
