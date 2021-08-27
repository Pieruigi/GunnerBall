using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class Ball : MonoBehaviourPun
    {

        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // Local ( should be the MasterClient )
            {
                stream.SendNext(PhotonNetwork.Time);
                stream.SendNext(rb.position);
                stream.SendNext(rb.rotation);
                stream.SendNext(rb.velocity);
                Debug.LogFormat("Ball - Sending sync [Time:{0}].", PhotonNetwork.Time);
            }
            else
            {
                double time = (double)stream.ReceiveNext();
                Vector3 pos = (Vector3)stream.ReceiveNext();
                Vector3 rot = (Vector3)stream.ReceiveNext();
                Vector3 vel = (Vector3)stream.ReceiveNext();
                Debug.LogFormat("Ball - Receiving sync [Time:{0}].", time);
            }
        }
    }

}
