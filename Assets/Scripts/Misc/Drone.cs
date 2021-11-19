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
        Transform pathGroup;

        [SerializeField]
        List<Transform> screws;

        [SerializeField]
        float rotationSpeed = 10f;

        List<Vector3>[] pathArray;

        Tween tween;

        int currentPathIndex = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            if (pathGroup)
            {
                // Create the path array
                pathArray = new List<Vector3>[pathGroup.childCount];

                // For each path fill the waypoint list
                for (int i = 0; i < pathArray.Length; i++)
                {
                    // Init the waypoint list
                    pathArray[i] = new List<Vector3>();

                    // Fill in the list
                    Transform path = pathGroup.GetChild(i);
                    for (int j = 0; j < path.childCount; j++)
                        pathArray[i].Add(path.GetChild(j).position);
                }


                List<Vector3> wps = pathArray[currentPathIndex];
                tween = gameObject.transform.DOPath(wps.ToArray(), 20, PathType.CatmullRom, PathMode.Full3D).OnComplete(HandleOnPathComplete);
            }
           
            
            foreach(Transform screw in screws)
            {
                // Set a random starting angle
                screw.RotateAround(screw.position, screw.up, Random.Range(-180f, 180f));
                
            }
            
            //tween.SetEase(Ease.InOutBack);

        }

        // Update is called once per frame
        void Update()
        {
            foreach (Transform screw in screws)
            {
                // Set a random starting angle
                screw.RotateAround(screw.position, screw.up, rotationSpeed*Time.deltaTime);

            }
        }

        void HandleOnPathComplete()
        {

        }
    }

}
