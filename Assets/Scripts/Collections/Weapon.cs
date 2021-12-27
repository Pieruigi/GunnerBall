using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Weapon : ScriptableObject
    {

        public static readonly string CollectionFolder = ResourceFolder.Collections + "/Weapons";

        //public static readonly string GameAssetFolder = ResourceFolder.GameAssets + "/Weapons";

        //[SerializeField]
        //GameObject gameAsset;

        
        //public GameObject GameAsset
        //{
        //    get { return gameAsset; }
        //}

        
        public Sprite Icon
        {
            get { return icon; }
        }

        [SerializeField]
        Sprite icon;
    }

}
