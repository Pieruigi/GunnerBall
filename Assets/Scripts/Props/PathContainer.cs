using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class PathContainer : MonoBehaviour
    {
        [System.Serializable]
        public class Path
        {
            public float Time
            {
                get { return time; }
            }

            public List<Transform> Waypoints
            {
                get { return waypoints; }
            }

            [SerializeField]
            float time = 30; // The time it will take to move along the path

            [SerializeField]
            List<Transform> waypoints;

        }

        //[SerializeField]
        //Transform waypoints;

        [SerializeField]
        List<Path> paths;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Path GetPath(int index)
        {
            return paths[index];
        }

        public int PathCount()
        {
            return paths.Count;
        }
    }

}
