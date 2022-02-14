using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.Interfaces
{
    public interface IPickable
    {
        void PickUp(GameObject picker);

        bool CanBePicked(GameObject picker);
    }

}
