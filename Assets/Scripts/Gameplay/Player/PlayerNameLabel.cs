
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zoca.UI
{
    public class PlayerNameLabel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text blueText;

        [SerializeField]
        TMP_Text redText;

        [SerializeField]
        Transform target;

        TMP_Text nameText;
        PlayerController owner;
        Camera localCamera;


        private void Awake()
        {

            owner = GetComponentInParent<PlayerController>();
            if (owner == PlayerController.Local)
                gameObject.SetActive(false);


        }

        // Start is called before the first frame update
        void Start()
        {
            // Reset both the text lables
            blueText.gameObject.SetActive(false);
            redText.gameObject.SetActive(false);

            // Get the player team color
            Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(owner.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
            // Set the text label depending on the team
            nameText = team == Team.Blue ? blueText : redText;

            // Set the player name
            nameText.text = owner.photonView.Owner.NickName;
            // Set the text field active
            nameText.gameObject.SetActive(true);
            // Get the local player camera
            localCamera = PlayerController.Local.PlayerCamera.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void LateUpdate()
        {
            // If the camera is not looking at the player then disable the label
            Vector3 dir = owner.transform.position - localCamera.transform.position;
            dir.y = 0;
            if(Vector3.Dot(dir, PlayerController.Local.transform.forward) < 0)
            {
                if(nameText.gameObject.activeSelf)
                    nameText.gameObject.SetActive(false);
            }
            else
            {
                if(!nameText.gameObject.activeSelf)
                    nameText.gameObject.SetActive(true);

                // Project on the screen
                Vector3 point = localCamera.WorldToScreenPoint(target.position);
                Debug.Log("Point:" + point);
                //RectTransform rt = nameText.transform as RectTransform;
                (nameText.transform as RectTransform).anchoredPosition = point;
            }
            

        }
    }

}
