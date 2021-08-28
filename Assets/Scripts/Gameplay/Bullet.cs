using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class Bullet : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //if (!PhotonNetwork.IsMasterClient)
            //    GetComponent<Collider>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }

}
