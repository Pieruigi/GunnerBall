using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Map : ScriptableObject
    {
        public static readonly string CollectionFolder = ResourceFolder.Collections + "/Maps";

        
        public int Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return _name; }
        }

        [SerializeField]
        int id;

        [SerializeField]
        string _name;
    }

}
