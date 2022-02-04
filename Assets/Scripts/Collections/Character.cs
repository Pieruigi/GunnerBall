using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Character : ScriptableObject
    {
        #region constants

        public static readonly string CollectionFolder = ResourceFolder.Collections + "/Characters";

        public static readonly string GameAssetFolder = ResourceFolder.GameAssets + "/Characters";
        #endregion

        #region properties
        public Sprite Avatar
        {
            get { return avatar; }
        }

        public GameObject GameAsset
        {
            get { return gameAsset; }
        }

        public IList<Weapon> Weapons
        {
            get { return weapons.AsReadOnly(); }
        }

        public float Speed
        {
            get { return gameAsset.GetComponent<PlayerController>().MaxSpeed; }
        }

        public float Stamina
        {
            get { return gameAsset.GetComponent<PlayerController>().Stamina; }
        }

        public float FreezingCooldown
        {
            get { return gameAsset.GetComponent<PlayerController>().FreezingCooldown; }
        }
        #endregion

        #region private fields

        [SerializeField]
        Sprite avatar;

        [SerializeField]
        string _name;

        [SerializeField]
        GameObject gameAsset;

        [SerializeField]
        List<Weapon> weapons;
        #endregion
    }

}
