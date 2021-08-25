using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class OfflineTester : MonoBehaviour
    {
        [SerializeField]
        GameObject gameManagerPrefab;

        private void Awake()
        {
            if (!GameManager.Instance)
            {
                PhotonNetwork.OfflineMode = true;
                Instantiate(gameManagerPrefab);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
