using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class RoomObjectLifeTime : MonoBehaviour
    {
        [SerializeField]
        float lifeTime = 20;

        bool networked = false;

        private void Awake()
        {
            if (GetComponent<PhotonView>())
                networked = true;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            lifeTime -= Time.deltaTime;

            // Only the master client can network destroy the object
            if (PhotonNetwork.IsMasterClient || !networked)
            {
                if(lifeTime < 0)
                {
                    if (!networked)
                        Destroy(gameObject);
                    else
                        PhotonNetwork.Destroy(gameObject);
                }

            }
        }
    }

}
