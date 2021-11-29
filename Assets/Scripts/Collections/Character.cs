using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Collections
{
    public class Character : ScriptableObject
    {
        [SerializeField]
        string id;

        [SerializeField]
        string _name;

        [SerializeField]
        GameObject asset;
    }

}
