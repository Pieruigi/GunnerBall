using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Zoca.Interfaces
{
    public interface IPickable
    {
        event UnityAction<IPickable, GameObject> OnPicked;

        void PickUp(GameObject picker);

        bool CanBePicked(GameObject picker);
    }

}
