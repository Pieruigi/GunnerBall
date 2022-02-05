using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum SpawnableType { PowerUp, PowerDown, Quaker, Throwable }

    /// <summary>
    /// We have four types of spawnable objects:
    /// - powerup: increases a skill of the player who pick it up ( common )
    /// - powerdown: decreases a skill of the opposite team ( less common than powerup )
    /// - quakeup: powerfull powerup that increases one/two skill/s of the player who pick it up ( rare )
    /// - throwable: granades with different effect.
    /// When you pick up a power up or a throwable the corresponding power up or throwable of the same type,
    /// if its active on the player, will be replaced.
    /// 
    /// </summary>
    public class SpawnManager : MonoBehaviourPunCallbacks
    {
     
        #region properties
        public static SpawnManager Instance { get; private set; }
        #endregion

        #region private fields
        [Header("General Section")]
        /// <summary>
        /// The maximum number of powerup spawned at the same time in the arena.
        /// </summary>
        [SerializeField]
        int spawnableMaximumNumber = 4;

            

        /// <summary>
        /// How time time it will take for the next powerup to spawn.
        /// </summary>
        [SerializeField]
        float respawnTime = 20;

        [SerializeField]
        List<Transform> spawnPoints = new List<Transform>();

        /// <summary>
        /// 0: powerup
        /// 1: powerdown
        /// 2: quakeup
        /// 3: throwable
        /// </summary>
        [SerializeField]
        int[] weights = new int[4];

    
        /// <summary>
        /// All the skills you can power up given a specific character and weapon ( speed, fireRate, ecc... )
        /// </summary>
        int numOfSkills = 6;

        /// <summary>
        /// If true powerups spawn at every kickoff.
        /// </summary>
        bool spawnOnKickOff = false;

        int[] spawnableIds;
        #endregion



        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                ResetAll();

                // Create the weight array
                FillWeightArray();

            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) 
            {
                // We create spawnables all at once on start
                for(int i=0; i<spawnableMaximumNumber; i++)
                {
                    CreateSpawnable();
                }
            }
            else // Not the master client
            {

            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ResetAll()
        {

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint.childCount > 0)
                    Destroy(spawnPoint.GetChild(0).gameObject);
            }
            
        }




      
        void FillWeightArray()
        {
            int count = 0;
            for(int i=0; i<weights.Length; i++)
            {
                count += (int)weights[i];
            }

            spawnableIds = new int[count];
            Debug.Log("WeightArray.Length:" + spawnableIds.Length);

            int start = 0, length = 0;
            for(int i=0; i<weights.Length; i++)
            {
                start = length;
                length += (int)weights[i];
                for (int j = start; j < length; j++)
                    spawnableIds[j] = i;
            }

            Debug.LogFormat("NumberOfSpawnable - {0},{1},{2},{3}",
                new List<int>(spawnableIds).FindAll(i=>i==0).Count,
                new List<int>(spawnableIds).FindAll(i => i == 1).Count,
                new List<int>(spawnableIds).FindAll(i => i == 2).Count,
                new List<int>(spawnableIds).FindAll(i => i == 3).Count);
            
        }

        void CreateSpawnable()
        {
            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                // We implement the spawning logic in the master client.
                // Get the next spawnable item depending on its weight
                int spawnableId = spawnableIds[Random.Range(0, spawnableIds.Length)];
                // Get a free spawn point
                int spawnPointId = GetFreeSpawnPointId(spawnableId);

                // Create a new item
                Debug.LogFormat("New Spawnable created - itamId:{0}, spawnPointId:{1}", spawnableId, spawnPointId);

                GameObject g = new GameObject("Item");
                g.transform.parent = spawnPoints[spawnPointId];
                // Update room custom properties             
                //PhotonNetwork.CurrentRoom.CustomProperties.Remove
            }
            else // No the master client
            {
                // Get custom property data
            }
        }

        int GetFreeSpawnPointId(int spawnableId)
        {
            
            // Get all the empty spawn points
            List<Transform> tmp = spawnPoints.FindAll(s => s.childCount == 0);

            // Return the id of a random empty spawn point 
            return spawnPoints.IndexOf(tmp[Random.Range(0, tmp.Count)]);
        }
        #endregion
    }

}
