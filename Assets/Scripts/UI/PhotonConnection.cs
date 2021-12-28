using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class PhotonConnection : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameObject panel;

        [SerializeField]
        Transform image;

        private void Awake()
        {
            panel.SetActive(true);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                image.Rotate(new Vector3(0, 0, 100 * Time.deltaTime));
            }
        }

        public override void OnConnectedToMaster()
        {
            panel.SetActive(false);
        }
    }

}
