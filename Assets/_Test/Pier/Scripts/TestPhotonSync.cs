using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestPhotonSync : MonoBehaviour, IPunObservable
{
    int state = 0;

    float elapsed = 0;

    private void Awake()
    {
        Debug.LogFormat("TestPhotonSync - Awake() state:{0}", state);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogFormat("TestPhotonSync - Start() state:{0}", state);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        elapsed += Time.deltaTime;

        if(elapsed > 10)
        {
            elapsed = 0;
            state++;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            
            stream.SendNext((byte)state);
            Debug.LogFormat("TestPhotonSync - Sending state: {0}", state);
        }
        else
        {
            state = (byte)stream.ReceiveNext();
            Debug.LogFormat("TestPhotonSync - Receiving state: {0}", state);
        }
    }

}
