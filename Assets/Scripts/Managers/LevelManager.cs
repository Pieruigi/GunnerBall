using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class LevelManager : MonoBehaviour, IOnEventCallback
    {
        #region constants
        public const string RoomObjectsResourceFolder = "GameAssets/RoomObjects";
        #endregion

        #region properties
        public static LevelManager Instance { get; private set; }

        public IList<Transform> BlueTeamSpawnPoints
        {
            get { return blueTeamSpawnPoints.AsReadOnly(); }
        }

        public IList<Transform> RedTeamSpawnPoints
        {
            get { return redTeamSpawnPoints.AsReadOnly(); }
        }

        public Transform BallSpawnPoint
        {
            get { return ballSpawnPoint; }
        }
        #endregion

        #region private fields
        [Header("Main Section")]
        [SerializeField]
        List<Transform> blueTeamSpawnPoints;
        

        [SerializeField]
        List<Transform> redTeamSpawnPoints;
        

        [SerializeField]
        Transform ballSpawnPoint;

        [Header("Room Objects Section")]
        [SerializeField]
        GameObject barrierPrefab;

        [SerializeField]
        GameObject electricGrenadePrefab;
        #endregion

        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                //Physics.gravity = Vector3.down * 12;
            }
            else
            {
                Destroy(gameObject);
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

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        
        #endregion

        #region photon event callback
        public void OnEvent(EventData photonEvent)
        {   
            switch (photonEvent.Code)
            {
                case PhotonEvent.SpawnBarrier:

                    // Only the master client can spawn networked room objects
                    //if (PhotonNetwork.IsMasterClient)
                    //{
                        // Get the index of the object to be spawned
                        object[] data = (object[])photonEvent.CustomData;
                        //string prefabName = (string)data[0];
                        Vector3 position = (Vector3)data[0];
                        Quaternion rotation = (Quaternion)data[1];
                        //PhotonNetwork instantiate
                        //string path = System.IO.Path.Combine(RoomObjectsResourceFolder, prefabName);
                        Instantiate(barrierPrefab, position, rotation);
                    //}
                    break;
                case PhotonEvent.SpawnElectricGrenade:
                    //if (PhotonNetwork.IsMasterClient)
                    //{
                        // Get the index of the object to be spawned
                        data = (object[])photonEvent.CustomData;
                        //prefabName = (string)data[0];
                        position = (Vector3)data[0];
                        rotation = (Quaternion)data[1];
                        byte team = (byte)data[2];
                        //PhotonNetwork instantiate
                        //path = System.IO.Path.Combine(RoomObjectsResourceFolder, prefabName);
                        GameObject g = Instantiate(electricGrenadePrefab, position, rotation);
                        g.GetComponent<ElectricGrenade>().SetTargetTeam((int)team);
                    //}
                    break;
            }
        }
        #endregion

        #region public methods
        public void SpawnBarrier(Vector3 position, Quaternion rotation)
        {
            object[] content = new object[] { position, rotation };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.SpawnBarrier, content, options, SendOptions.SendReliable);
            
        }

        public void SpawnElectricGrenade(Vector3 position, Quaternion rotation, int targetTeam)
        {
            object[] content = new object[] { position, rotation, (byte)targetTeam };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.SpawnElectricGrenade, content, options, SendOptions.SendReliable);
            
        }
        #endregion
    }

}
