using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Character : ScriptableObject
    {

        public static readonly string CollectionFolder = ResourceFolder.Collections + "/Characters";

        public static readonly string GameAssetFolder = ResourceFolder.GameAssets + "/Characters";

        public Sprite Avatar
        {
            get { return avatar; }
        }

        public GameObject GameAsset
        {
            get { return gameAsset; }
        }

        [SerializeField]
        Sprite avatar;

        [SerializeField]
        string _name;

        [SerializeField]
        GameObject gameAsset;

        
        
    }

}
