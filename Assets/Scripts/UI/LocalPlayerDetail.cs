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

        // Start is called before the first frame update
        void Start()
        {
            playerText.text = AccountManager.Instance.PlayerName;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
