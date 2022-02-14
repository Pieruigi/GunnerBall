using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField]
        List<Transform> blueTeamSpawnPoints;
        public IList<Transform> BlueTeamSpawnPoints
        {
            get { return blueTeamSpawnPoints.AsReadOnly(); }
        }

        [SerializeField]
        List<Transform> redTeamSpawnPoints;
        public IList<Transform> RedTeamSpawnPoints
        {
            get { return redTeamSpawnPoints.AsReadOnly(); }
        }

        [SerializeField]
        Transform ballSpawnPoint;
        public Transform BallSpawnPoint
        {
            get { return ballSpawnPoint; }
        }

        [SerializeField]
        GameObject barrierPrefab;
        public GameObject BarrierPrefab
        {
            get { return barrierPrefab; }
        }

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

        
    }

}
