using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Weapon : ScriptableObject
    {
        #region constants
        public static readonly string CollectionFolder = ResourceFolder.Collections + "/Weapons";

        //public static readonly string GameAssetFolder = ResourceFolder.GameAssets + "/Weapons";
        #endregion

        #region properties

        public Sprite Icon
        {
            get { return icon; }
        }

        public float FirePower
        {
            get { return gameAsset.GetComponent<FireWeapon>().Power; }
        }

        public float FireRate
        {
            get { return gameAsset.GetComponent<FireWeapon>().FireRate; }
        }

        public float FireRange
        {
            get { return gameAsset.GetComponent<FireWeapon>().FireRange; }
        }

        #endregion

        #region private fields
        [SerializeField]
        Sprite icon;

        [SerializeField]
        GameObject gameAsset;
        #endregion

        
    }

}
