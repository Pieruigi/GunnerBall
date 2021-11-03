using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class OfflinePlayer
    {
        Dictionary<string, object> customProperties;
        public Dictionary<string, object> CustomProperties
        {
            get { return customProperties; }
        }

        public OfflinePlayer()
        {
            customProperties = new Dictionary<string, object>();
        }
    }

}
