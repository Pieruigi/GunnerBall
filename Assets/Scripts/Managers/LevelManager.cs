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
        List<Transform> blueTeamSpawnGroup;

        [SerializeField]
        List<Transform> redTeamSpawnGroup;

        //[SerializeField]
        List<Transform> blueTeamSpawnPoints;
        

        //[SerializeField]
        List<Transform> redTeamSpawnPoints;
        


        [SerializeField]
        Transform ballSpawnPoint;

        [Header("Room Objects Section")]
        [SerializeField]
        GameObject barrierPrefab;

        [SerializeField]
        GameObject electricGrenadePrefab;

        [SerializeField]
        GameObject magnetPrefab;

        [field: Header("Misc")]
        [field: SerializeField]
        public GameObject StepParticlesPrefab { get; private set; }

        
        float spawnDelay = 0.5f;
        #endregion

        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                Debug.Log("LevelManager Awake");
                // Get the spawn point group by game mode ( 1vs1, 2vs2, ecc... )
                int index = PhotonNetwork.CurrentRoom.MaxPlayers / 2 - 1;
                blueTeamSpawnPoints = new List<Transform>();
                for(int i=0; i<blueTeamSpawnGroup[index].childCount; i++)
                {
                    blueTeamSpawnPoints.Add(blueTeamSpawnGroup[index].GetChild(i));
                }
                redTeamSpawnPoints = new List<Transform>();
                for (int i = 0; i < redTeamSpawnGroup[index].childCount; i++)
                {
                    redTeamSpawnPoints.Add(redTeamSpawnGroup[index].GetChild(i));
                }

            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //Match.Instance.OnStateChanged += () => { if (Match.Instance.State == (int)MatchState.Paused) UnspawnAll(); };
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

     
        IEnumerator DoSpawnElectricGrenade(Vector3 position, Quaternion rotation, int targetTeam, double time)
        {
            // Check how much time has passed and add the remaining delay
            float lag = (float)(PhotonNetwork.Time - time);
            if (spawnDelay >= lag)
                yield return new WaitForSeconds(spawnDelay - lag);

            GameObject g = Instantiate(electricGrenadePrefab, position, rotation);
            g.GetComponent<ElectricGrenade>().SetTargetTeam((int)targetTeam);
        }

        IEnumerator DoSpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, double time)
        {
            // Check how much time has passed and add the remaining delay
            float lag = (float)(PhotonNetwork.Time - time);
            if (spawnDelay >= lag)
                yield return new WaitForSeconds(spawnDelay - lag);

            Instantiate(prefab, position, rotation);
        }

        #endregion

        #region photon event callback
        public void OnEvent(EventData photonEvent)
        {   
            
            switch (photonEvent.Code)
            {
                case PhotonEvent.SpawnBarrier:

                    object[] data = (object[])photonEvent.CustomData;
                    Vector3 position = (Vector3)data[0];
                    Quaternion rotation = (Quaternion)data[1];
                    double time = (double)data[2];
                    StartCoroutine(DoSpawnPrefab(barrierPrefab, position, rotation, time));
                    break;
                case PhotonEvent.SpawnElectricGrenade:
                    data = (object[])photonEvent.CustomData;
                    position = (Vector3)data[0];
                    rotation = (Quaternion)data[1];
                    byte team = (byte)data[2];
                    time = (double)data[3];
                    StartCoroutine(DoSpawnElectricGrenade( position, rotation, (int)team, time));
                    break;
                case PhotonEvent.SpawnMagnet:
                    data = (object[])photonEvent.CustomData;
                    position = (Vector3)data[0];
                    rotation = (Quaternion)data[1];
                    time = (double)data[2];
                    StartCoroutine(DoSpawnPrefab(magnetPrefab, position, rotation, time));
                    break;
            }
        }
        #endregion

        #region public methods
        public void SendEventSpawnBarrier(Vector3 position, Quaternion rotation)
        {
            object[] content = new object[] { position, rotation, PhotonNetwork.Time };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.SpawnBarrier, content, options, SendOptions.SendReliable);
            
        }

        public void SendEventSpawnElectricGrenade(Vector3 position, Quaternion rotation, int targetTeam)
        {
            object[] content = new object[] { position, rotation, (byte)targetTeam, PhotonNetwork.Time };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.SpawnElectricGrenade, content, options, SendOptions.SendReliable);
            
        }

        public void SendEventSpawnMagnet(Vector3 position, Quaternion rotation)
        {
            object[] content = new object[] { position, rotation, PhotonNetwork.Time };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.SpawnMagnet, content, options, SendOptions.SendReliable);

        }

       

        #endregion
    }

}
