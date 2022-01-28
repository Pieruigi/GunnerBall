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
            //get { return id; }
            get { return int.Parse(name.Split('.')[0]); }
        }

        public string Name
        {
            get { return _name; }
        }

        public Sprite ImageSprite
        {
            get { return imageSprite; }
        }

        public GameObject Ball
        {
            get { return ball; }
        }

        //[SerializeField]
        //int id;

        [SerializeField]
        string _name;

        [SerializeField]
        Sprite imageSprite;

        [SerializeField]
        GameObject ball;
    }

}
