using Photon.Pun;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

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
    public class PickableManager : MonoBehaviourPunCallbacks
    {

     
        #region properties
        public static PickableManager Instance { get; private set; }

        const string PickablePrefabFolder = "Pickables";
        #endregion

        #region private fields
        [Header("General Section")]
        /// <summary>
        /// The maximum number of powerup spawned at the same time in the arena.
        /// </summary>
        [SerializeField]
        int pickableMaximumNumber = 4;

        [SerializeField]
        int pickableMinimumNumber = 2;


        /// <summary>
        /// How time time it will take for the next powerup to spawn.
        /// </summary>
        [SerializeField]
        float respawnTime = 60;

        [SerializeField]
        List<Transform> spawnPoints = new List<Transform>();

        /// <summary>
        /// Each weight correspond to a specific spawnable object.
        /// The id of the weight is the id of the object which is in the resource folder.
        /// </summary>
        [SerializeField]
        int[] weights;

        [SerializeField]
        List<GameObject> prefabs = new List<GameObject>();

        //[SerializeField]

        /// <summary>
        /// All the skills you can power up given a specific character and weapon ( speed, fireRate, ecc... )
        /// </summary>
        int numOfSkills = 6;

        /// <summary>
        /// If true powerups spawn at every kickoff.
        /// </summary>
        bool spawnOnKickOff = false;

        int[] pickableIds;

        /// <summary>
        /// 0: spawnable id
        /// 1: spawnable subid
        /// 2: spawn point id
        /// </summary>
        
        string keyFormat = "sp_{0}_{1}";
        
        
        List<Transform> takenSpawnPoints = new List<Transform>();

        float respawnCooldown = 0;
        #endregion



        #region private methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                ResetAll(); // Don't really need it

              
                // Only the master client should fill the spawnable array
                if (PhotonNetwork.IsMasterClient)
                {
                    // Fill the array
                    FillPickableArray();

                    // Adjust minimum and maximum depending on the number of players
                    pickableMinimumNumber *= PhotonNetwork.CurrentRoom.MaxPlayers;
                    pickableMaximumNumber *= PhotonNetwork.CurrentRoom.MaxPlayers;
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

            Match.Instance.OnStateChanged += HandleOnStateChanged;

            if (PhotonNetwork.IsMasterClient)
            {
                respawnCooldown = respawnTime;
                int count = Random.Range(pickableMinimumNumber, pickableMaximumNumber + 1);
                // We create spawnables all at once on start
                for (int i = 0; i < count; i++)
                {
                    CreatePickable();
                }
            }
            else // Not the master client
            {
                foreach (object key in PhotonNetwork.CurrentRoom.CustomProperties.Keys)
                {
                    if (key.ToString().StartsWith("sp_"))
                    {
                        Debug.LogFormat("Fount a spawnable key " + key);
                        string[] splits = key.ToString().Split('_');
                        InstantiatePickableObject(int.Parse(splits[1]), int.Parse(splits[2]));
                    }
                    
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            // The master client takes care of the respawn
            if (PhotonNetwork.IsMasterClient)
            {
                if(Match.Instance.State == (int)MatchState.Started)
                {
                    
                    

                    // Update cooldown
                    respawnCooldown -= Time.deltaTime;

                    // It's time to respawn pickables
                    if(respawnCooldown < 0 || spawnOnKickOff)
                    {
                        if(spawnOnKickOff)
                            spawnOnKickOff = false;
                        
                        // Set the cooldown for the next time
                        if(respawnCooldown < 0)
                            respawnCooldown = respawnTime;
                        
                        

                        // Get the current number of pickables
                        int count = 0;
                        
                        for (int i = 0; i < spawnPoints.Count; i++)
                        {
                            if (spawnPoints[i].childCount > 0)
                                count++;
                        }

                        // If it's less than the maximum number then we could spawn some others
                        if (count < pickableMaximumNumber)
                        {
                            // Lets check if we need more pickables
                            int r = Random.Range(pickableMinimumNumber, pickableMaximumNumber + 1);
                            if(r>count)
                            {
                                count = r - count;
                                for (int i = 0; i < count; i++)
                                    CreatePickable();
                            }
                        }
                    }
                    
                }


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


       void HandleOnStateChanged()
        {
            if(Match.Instance.State == (int)MatchState.Goaled)
            {
                spawnOnKickOff = true;
            }
        }


        void FillPickableArray()
        {
            int count = 0;
            for(int i=0; i<weights.Length; i++)
            {
                count += (int)weights[i];
            }

            pickableIds = new int[count];
          
            int start = 0, length = 0;
            for(int i=0; i<weights.Length; i++)
            {
                start = length;
                length += (int)weights[i];
                for (int j = start; j < length; j++)
                    pickableIds[j] = i;
            }

            for(int i=0; i<weights.Length; i++)
            {
                Debug.LogFormat("SpwnableId:{0}, Count:{1}", i, new List<int>(pickableIds).FindAll(s => s == i).Count);
            }
            
        }

        /// <summary>
        /// Objects are spawned by the master client.
        /// </summary>
        /// <param name="pickableId"></param>
        /// <param name="spawnPointId"></param>
        void InstantiatePickableObject(int pickableId, int spawnPointId)
        {
            // Create a new item
            Debug.LogFormat("New Spawnable created - itemId:{0}, spawnPointId:{1}", pickableId, spawnPointId);

            // Get the spawnable object by the id
            GameObject prefab = prefabs[pickableId];

            // Get the spawn point
            Transform spawnPoint = spawnPoints[spawnPointId];

            // Instantiate
            GameObject g = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            g.name = pickableId.ToString();

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
        void CreatePickable()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            // We implement the spawning logic in the master client.
            // Get the next spawnable item depending on its weight
            int pickableId = pickableIds[Random.Range(0, pickableIds.Length)];
            // Get a free spawn point
            int spawnPointId = GetFreeSpawnPointId(pickableId);
            // Set the spawn point taken
            takenSpawnPoints.Add(spawnPoints[spawnPointId]);
            

            // Add the new custom property
            ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
            ht.Add(string.Format(keyFormat, pickableId, spawnPointId), (int)0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            
            
        }

        int GetFreeSpawnPointId(int spawnableId)
        {
            List<int> tmp = new List<int>();
            for(int i=0; i<spawnPoints.Count; i++)
            {
                if (!takenSpawnPoints.Contains(spawnPoints[i]))
                    tmp.Add(i);
            }

            // Return the id of a random empty spawn point 
            return tmp[Random.Range(0, tmp.Count)];
        }

        

        #endregion

        #region pun callbacks
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            Debug.LogFormat("CustomProperties.Count:{0}", PhotonNetwork.CurrentRoom.CustomProperties.Count);
            Debug.Log("PropertyChanged.Count:" + propertiesThatChanged.Count);

            // Loop through all the properties that have changed
            foreach (object key in propertiesThatChanged.Keys)
            {
                Debug.Log("Key:" + key.ToString());
               
                    
                // Check if the key that changed refers to a spawnable item
                if (key.ToString().StartsWith("sp_"))
                {
                    // It's a spawnable property.
                    Debug.LogFormat("Spawnable updated - {0}", key.ToString());

                    // Parse the key as string
                    string propKey = key.ToString();

                    // Read the value:
                    // 0 - spawnable just initialized, we must create the physical object
                    // >0 - it is the actor number who picked up the spawnable
                    // null - property has been removed, means the spawnable has been activated 
                    object o = null;
                    int propValue = 0;
                    if (propertiesThatChanged.TryGetValue(key.ToString(), out o))
                    {
                        if (o != null)
                        {
                            // Value is not null, so the spawnable has been just initialized or picked.
                            Debug.LogFormat("Found value for {0}:{1}", key.ToString(), o.ToString());
                            propValue = (int)o;
                        }
                        else
                        {
                            // Value is null, one of the players picked up the spawnable
                            Debug.LogFormat("Value for {0} is null", key.ToString());
                            
                        }
                            
                    }

                    // Value == null means that the key has been removed ( to remove a key you just need 
                    // to call SetCustomProperty on that key by passing null as value ).
                    if (o == null)
                        continue;

                    // Key has not been removed, so the spawnable is alive.
                    // Get spawnable detail
                    string[] propSplits = propKey.Split('_');
                    int pickableId = int.Parse(propSplits[1]);
                    int spawnPointId = int.Parse(propSplits[2]);

                    if (propValue == 0) // Has just been initialized
                    {
                        // Each client instantiate spawnable objects
                        // We wait for the server to respond ( even in the master client )
                        // to take care of the lag.
                        InstantiatePickableObject(pickableId, spawnPointId);
                    }
                    else // Spawnable has been picked up by a player or the AI
                    {
                        // The property value is not zero, this means that someone took the spawnable.
                        if(propValue == PhotonNetwork.LocalPlayer.ActorNumber || PhotonNetwork.OfflineMode)
                        {
                            // Every client executes his own code when the owned player picks up a spawnable 
                            // object.
                            // In offline mode there is only one human player and some AIs, so its always 
                            // the local client executing the code.
                            // Get the player controller
                            PlayerController picker = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>()).Find(p => p.photonView.CreatorActorNr == propValue);
                            // Pick up the object
                            spawnPoints[spawnPointId].GetComponentInChildren<IPickable>().PickUp(picker.gameObject);
                            
                            // Remove the properties
                            PhotonNetwork.CurrentRoom.SetCustomProperties(
                                new ExitGames.Client.Photon.Hashtable() { { propKey, null } }  );

                        }
                        else
                        {
                            // Not in offline mode and the local player is not the one who's 
                            // picking up the spawnable object.
                            // We could find the player who picked up the spawnable object and nay apply
                            // some effect.
                        }

                        // Destroy the object anyway, no matter who is picking the object.
                        SetSpawnPointFree(spawnPointId);
                        
                    }
                    

                }

            }


        }

        void SetSpawnPointFree(int spawnPointId)
        {
            if (spawnPoints[spawnPointId].childCount > 0)
            {
                // Destroy the physical object
                GameObject item = spawnPoints[spawnPointId].GetChild(0).gameObject;
                Destroy(item);

                // Set free
                takenSpawnPoints.Remove(spawnPoints[spawnPointId]);

            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Local player only.
        /// In online matches it is called only by the player who tries to pick up the spawnable.
        /// In offline matches is the always the master client who call the 
        /// </summary>
        /// <param name="pickable"></param>
        /// <param name="actorNumber"></param>
        public void TryPickUp(GameObject pickable, int actorNumber)
        {
            //if (Match.Instance.State == (int)MatchState.Goaled)
            //    return;

            Debug.Log("Trying to pick up " + pickable + ", actorNumber:" + actorNumber);

            // We go ahead only if the local player is the one trying to pick up the object or if we are
            // playing in offline mode ( so we are the master client and we need to take care of the AI )
            if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber && !PhotonNetwork.OfflineMode)
                return;

            // Get the spawn point id
            int spawnPointId = spawnPoints.IndexOf(pickable.transform.parent);

            // Pickable name has the format id.name ( ex: 01.speedUp )               
            //string pickableName = pickable.name.Split('.')[0];
            
            int pickableId = int.Parse(pickable.name);
            
          
            // Build the property key
            string key = string.Format(keyFormat, pickableId, spawnPointId);
            
            // Try to check and swap the corresponding custom property.
            // Property different than zero means that someone else has already picked up the object and we
            // just need to wait for the server to send the update callback.
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable() { { key, actorNumber } },
                new ExitGames.Client.Photon.Hashtable() { { key, 0 } }
                );

        }
        #endregion
    }

}
