using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca
{
    public class PlayerNameLabel : MonoBehaviour
    {

        TMP_Text nameText;
        PlayerController owner;
        PlayerCamera localCamera;

        Color blue = new Color(0, 0, 0.75f);
        Color red = new Color(0.75f, 0, 0);

        private void Awake()
        {
            owner = GetComponentInParent<PlayerController>();
            if (owner == PlayerController.Local)
                gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            nameText = GetComponentInChildren<TMP_Text>();

            nameText.text = owner.photonView.Owner.NickName;

            localCamera = PlayerController.Local?.PlayerCamera;

            // Team color
            Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(owner.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
            nameText.color = team == Team.Blue ? blue : red;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void LateUpdate()
        {
            if(!localCamera)
                localCamera = PlayerController.Local?.PlayerCamera;

            if (!localCamera)
                return;

            Vector3 dir = localCamera.transform.position - nameText.transform.position;
            nameText.transform.forward = -dir.normalized;
        }
    }

}
