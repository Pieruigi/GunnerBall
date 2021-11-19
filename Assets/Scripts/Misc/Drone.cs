using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;

namespace Zoca
{
    public class Drone : MonoBehaviour
    {
        [SerializeField]
        Transform waypointGroup;

        
        // Start is called before the first frame update
        void Start()
        {
            List<Vector3> wps = new List<Vector3>();

            for (int i=0; i<waypointGroup.childCount; i++)
                wps.Add(waypointGroup.GetChild(i).position);

            // Add the first to close the path
            //wps.Add(wps[0]);

            Tween t = gameObject.transform.DOPath(wps.ToArray(), 20, PathType.CatmullRom, PathMode.Full3D);
            t.SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
