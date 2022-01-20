using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Collections;

namespace Zoca
{
    public class MapManager : MonoBehaviour
    {

        public static MapManager Instance { get; private set; }

        List<Map> maps;
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                // Load maps from resources
                maps = new List<Map>(Resources.LoadAll<Map>(Map.CollectionFolder));


                DontDestroyOnLoad(gameObject);
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

        #region public methods
        public Map GetMap(int mapId)
        {
            return maps.Find(m => m.Id == mapId);
        }

        public IList<Map> GetAvailableMaps()
        {
            return maps.AsReadOnly();
        }
        #endregion
    }

}
