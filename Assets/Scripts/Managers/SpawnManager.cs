using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    

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

        [Header("PowerUp Section")]
        [SerializeField]
        List<Transform> powerUpSpawnPoints; // Available powerup spawn points

        [SerializeField]
        [Range(0f, 1f)]
        float powerUpWeight = 1f;

        

        /// <summary>
        /// How time time it will take for the next powerup to spawn.
        /// </summary>
        [SerializeField]
        float spawnTime = 20;

        [Header("PowerDown Section")]
        [SerializeField]
        List<Transform> powerDownSpawnPoints; // Available powerdown spawn points

        [SerializeField]
        [Range(0f, 1f)]
        float powerDownWeight = .4f;

        [Header("QuakeUp Section")]
        [SerializeField]
        List<Transform> quakeUpSpawnPoints; // Available quakeup spawn points

        [SerializeField]
        [Range(0f, 1f)]
        float quakeUpWeight = .1f;

        [Header("Throwable Section")]
        [SerializeField]
        List<Transform> throwableSpawnPoints; // Available throwable spawn points

        [SerializeField]
        [Range(0f, 1f)]
        float throwableWeight = .5f;

        /// <summary>
        /// All the skills you can power up given a specific character and weapon ( speed, fireRate, ecc... )
        /// </summary>
        int numOfSkills = 6;

        /// <summary>
        /// If true powerups spawn at every kickoff.
        /// </summary>
        bool spawnOnKickOff = false;

        /// <summary>
        /// 0: powerup
        /// 1: powerdown
        /// 2: quakeup
        /// 3: throwable
        /// </summary>
        int[] weightArray;
        
        
        
        
        
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
            if (PhotonNetwork.IsMasterClient) 
            {
                
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
            PowerUpResetAll();
            PowerDownResetAll();
        }

        void PowerUpResetAll()
        {
            Debug.Log("PowerUp reset all");
            foreach(Transform t in powerUpSpawnPoints)
            {
                if (t.childCount > 0)
                    Destroy(t.GetChild(0).gameObject);
            }
        }

        void PowerDownResetAll()
        {
            Debug.Log("PowerDown reset all");
            foreach (Transform t in powerDownSpawnPoints)
            {
                if (t.childCount > 0)
                    Destroy(t.GetChild(0).gameObject);
            }
        }

        void CreateNewPowerUp()
        {

        }

        void FillWeightArray()
        {
            powerUpWeight *= 100;
            powerDownWeight *= 100;
            quakeUpWeight *= 100;
            throwableWeight *= 100;

            weightArray = new int[(int)(powerUpWeight + powerDownWeight + quakeUpWeight + throwableWeight)];
            Debug.Log("WeightArray.Length:" + weightArray.Length);

            int i = 0;
            //new ArrayList().AddRange()
            for(; i<powerUpWeight; i++)
                weightArray[i] = 0;
            for (; i < powerDownWeight; i++)
                weightArray[i] = 1;
            for (; i < quakeUpWeight; i++)
                weightArray[i] = 2;
            for (; i < throwableWeight; i++)
                weightArray[i] = 3;

            // Debug array
            i = 0;
            int pup = 0, pdo = 0, qua = 0, thr = 0;
            for(; i<weightArray.Length; i++)
            {
                if (i == 0)
                    pup++;
                if (i == 1)
                    pdo++;
                if (i == 2)
                    qua++;
                if (i == 3)
                    thr++;
            }
            Debug.LogFormat("NumberOfSpawnable - {0},{1},{2},{3}",new List<int>(weightArray).FindAll(i=>i==0).Count,pdo, qua,thr);
            
        }
        #endregion
    }

}
