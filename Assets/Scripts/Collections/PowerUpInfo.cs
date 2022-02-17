using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca.Collections
{
    public class PowerUpInfo : ScriptableObject
    {
        #region constants
        public static readonly string CollectionFolder = ResourceFolder.Collections + "/PowerUps";
        #endregion

        #region properties
        public Sprite Icon
        {
            get { return icon; }
        }
        #endregion

        #region private fields
        [SerializeField]
        string _name;

        [SerializeField]
        Sprite icon;

        [SerializeField]
        GameObject powerUpAsset;
        #endregion

        #region public methods
        public System.Type GetPowerUpType()
        {
            return powerUpAsset.GetComponent<IPowerUp>().GetType();
        }
        #endregion
    }

}
