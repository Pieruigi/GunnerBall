
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class PlayerNameLabel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text nameText;

        [SerializeField]
        Transform target;

        PlayerController owner;
        Camera localCamera;

        Color blueColor = Color.blue;
        Color redColor = Color.red;
        Transform bg;
        bool hidden = false;

        private void Awake()
        {

            owner = GetComponentInParent<PlayerController>();
            if (owner == PlayerController.Local)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SettingsManager.Instance.OnHideNicknameChanged += delegate ()
                {

                    Show(!SettingsManager.Instance.HideNickname);
                };
            }
            
        }

        // Start is called before the first frame update
        void Start()
        {
           
            // Get the player team color
            Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(owner.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
            // Set the bg color
            bg = nameText.transform.parent;
            bg.GetComponent<Image>().color = team == Team.Blue ? blueColor : redColor;
            
            // Set the player name
            nameText.text = owner.photonView.Owner.NickName;
            // Set the text field active
            //nameText.gameObject.SetActive(true);
            // Get the local player camera
            localCamera = PlayerController.Local.PlayerCamera.GetComponent<Camera>();

            Debug.Log("Text size before:" + (nameText.transform as RectTransform).rect);
            (nameText.transform as RectTransform).ForceUpdateRectTransforms();
            //Debug.Log("Text size after:" + (nameText.transform as RectTransform).rect);
            StartCoroutine(ResizeBG());


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
                //if(nameText.gameObject.activeSelf)
                //    nameText.gameObject.SetActive(false);
                if (bg.gameObject.activeSelf)
                    bg.gameObject.SetActive(false);
            }
            else
            {
                //if(!nameText.gameObject.activeSelf)
                //    nameText.gameObject.SetActive(true);
                if (!bg.gameObject.activeSelf && !hidden)
                    bg.gameObject.SetActive(true);

                // Project on the screen
                Vector3 point = localCamera.WorldToScreenPoint(target.position);
                //Debug.Log("Point:" + point);
                //RectTransform rt = nameText.transform as RectTransform;
                (bg.transform as RectTransform).anchoredPosition = point;
            }
            

        }

        public void Show(bool value)
        {
            hidden = !value;
            bg.gameObject.SetActive(value);
                
        }

        IEnumerator ResizeBG()
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Text size after:" + (nameText.transform as RectTransform).rect);
            Vector2 size = (bg as RectTransform).sizeDelta;
            size.x = (nameText.transform as RectTransform).rect.width + 32f;
            (bg as RectTransform).sizeDelta = size;

            // Check for visibility
            Show(!SettingsManager.Instance.HideNickname);

        }
    }

}
