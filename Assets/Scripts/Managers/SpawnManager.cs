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

        /// <summary>
        /// 0: spawnable id
        /// 1: spawnable subid
        /// 2: spawn point id
        /// </summary>
        string keyFormat = "sp_{0}_{1}_{2}";

        //enum KeyValue { Pickable }
        //System.DateTime lastSpawnedTime;
        //int spawnableCount = 0;

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
                // We create spawnables all at once on start
                for (int i = 0; i < spawnableMaximumNumber; i++)
                {
                    InitSpawnable();
                }
            }
            else // Not the master client
            {
                foreach(object key in PhotonNetwork.CurrentRoom.CustomProperties.Keys)
                {
                    if(key.ToString().StartsWith("sp_"))
                    {
                        Debug.LogFormat("Fount a spawnable key " + key);
                        string[] splits = key.ToString().Split('_');
                        CreateSpawnable(int.Parse(splits[1]), int.Parse(splits[2]), int.Parse(splits[3]));
                    }
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Check if it's time to create another spawnable
                //if((System.DateTime.UtcNow - lastSpawnedTime).TotalSeconds > respawnTime)
                //{
                    
                //}
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                //PhotonNetwork.CurrentRoom.SetCustomProperties("Test_pup", 1);
                ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
                ht.Add("Test_pup", PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

                //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty("Test_pup", 1);
                //RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                object value = 0;
                if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Test_pup", out value))
                {
                    Debug.LogFormat("Property found Test_pup:{0}", value);
                }
                else
                {
                    Debug.LogFormat("Property not found Test_pup");
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
            {

                PhotonNetwork.CurrentRoom.CustomProperties.Remove("Test_pup");
                RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //PhotonNetwork.CurrentRoom.SetCustomProperties("Test_pup", 1);
                ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
                ht.Add("Test_pup", 154);
                ExitGames.Client.Photon.Hashtable eht = new ExitGames.Client.Photon.Hashtable();
                eht.Add("Test_pup", 11);
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht, eht);

                //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty("Test_pup", 1);
                //RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
            }
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

        /// <summary>
        /// All clients ( even the master client ).
        /// Creates the physical spawnable item.
        /// </summary>
        /// <param name="spawnableId"></param>
        /// <param name="spawnableSubId"></param>
        /// <param name="spawnPointId"></param>
        void CreateSpawnable(int spawnableId, int spawnableSubId, int spawnPointId)
        {
            // Create a new item
            Debug.LogFormat("New Spawnable created - itemId:{0}, itemSubId:{2}, spawnPointId:{1}", spawnableId, spawnPointId, spawnableSubId);

            GameObject g = new GameObject("Item");
            g.transform.parent = spawnPoints[spawnPointId];
        }

        /// <summary>
        /// Master client only.
        /// Inits a random spawnable and sets the corresponding room property.
        /// The name of the key holds the spawnable details such as the item type and the spawn point.
        /// The value is set to 0.
        /// When a player tries to pick up the item he checks for the corresponding key in the 
        /// room custom properties: if the key exists and it value is 0 the item can be picked.
        /// We use CAS to check the room custom properties.
        /// </summary>
        void InitSpawnable()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            //lastSpawnedTime = System.DateTime.UtcNow;
            
            // We implement the spawning logic in the master client.
            // Get the next spawnable item depending on its weight
            int spawnableId = spawnableIds[Random.Range(0, spawnableIds.Length)];
            // Get a free spawn point
            int spawnPointId = GetFreeSpawnPointId(spawnableId);

            int spawnableSubId = 0;
            // Add a new custom property
                
            ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
            ht.Add(string.Format(keyFormat, spawnableId, spawnableSubId, spawnPointId), (int)0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            
            
        }

        int GetFreeSpawnPointId(int spawnableId)
        {
            
            // Get all the empty spawn points
            List<Transform> tmp = spawnPoints.FindAll(s => s.childCount == 0);

            // Return the id of a random empty spawn point 
            return spawnPoints.IndexOf(tmp[Random.Range(0, tmp.Count)]);
        }

        

        #endregion

        #region pun callbacks
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            Debug.LogFormat("CustomProperties.Count:{0}", PhotonNetwork.CurrentRoom.CustomProperties.Count);
            Debug.Log("PropertyChanged.Count:" + propertiesThatChanged.Count);
            foreach (object key in propertiesThatChanged.Keys)
            {
                Debug.Log("Key:" + key.ToString());
                // Check if the key that changed refers to a spawnable item
                if (key.ToString().StartsWith("sp_"))
                {
                    Debug.LogFormat("Spawnable updated - {0}", key.ToString());

                    string propKey = key.ToString();
                    int propValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[propKey];

                    // Get spawnable detail
                    string[] propSplits = propKey.Split('_');
                    int spawnableId = int.Parse(propSplits[1]);
                    int spawnableSubId = int.Parse(propSplits[2]);
                    int spawnPointId = int.Parse(propSplits[3]);

                    if (propValue == 0) // Has just been initialized
                    {
                       
                        // Create the physical spawnable item.
                        CreateSpawnable(spawnableId, spawnableSubId, spawnPointId);
                    }
                    else // Someone picked it up, is that the local player?
                    {
                        if(propValue == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            // Use spawnable item on the local player
                        }
                        else
                        {
                            // Someone else
                        }

                        // Destroy the object anyway
                        // Get the spawn point
                        if(spawnPoints[spawnPointId].childCount > 0)
                        {
                            GameObject item = spawnPoints[spawnPointId].GetChild(0).gameObject;
                            Destroy(item);
                        }
                        
                    }
                    

                }

            }


        }
        #endregion
    }

}
