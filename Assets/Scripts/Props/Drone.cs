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
        PathContainer pathContainer;

        [SerializeField]
        float idleTimeMin = 10;

        [SerializeField]
        float idleTimeMax = 30;

        [SerializeField]
        float targetTimeMin = 100;

        [SerializeField]
        float targetTimeMax = 300;

        [SerializeField]
        List<Transform> screws;

        [SerializeField]
        float rotationSpeed = 10f;

        Tween tween;

        int currentPathIndex = 0;
        bool back = false;
        Transform target;
        
        // Start is called before the first frame update
        void Start()
        {
            // Start tweening
            Tween(GetWaypoints(pathContainer.GetPath(currentPathIndex), back), pathContainer.GetPath(currentPathIndex).Time, Random.Range(idleTimeMin, idleTimeMax));

            // Start the target updating coroutine
            StartCoroutine(UpdateTarget());

            // Set a random starting angle for each screw
            foreach (Transform screw in screws)
                screw.RotateAround(screw.position, screw.up, Random.Range(-180f, 180f));
            
            

        }

        // Update is called once per frame
        void Update()
        {
            // Rotate screws
            foreach (Transform screw in screws)
                screw.RotateAround(screw.position, screw.up, rotationSpeed*Time.deltaTime);

        }

        void HandleOnPathComplete()
        {
            // Set the next path
            if (!back) // Forward
            {
                if(currentPathIndex == pathContainer.PathCount() - 1)
                {
                    // Reverse the direction
                    back = true;
                }
                else
                {
                    // Keep going
                    currentPathIndex += 1;
                }
            }
            else // We are going back
            {
                if(currentPathIndex == 0)
                {
                    // Reverse the direction
                    back = false;
                }
                else
                {
                    // Keep going
                    currentPathIndex -= 1;
                }
            }

           
            // Start tweening
            Tween(GetWaypoints(pathContainer.GetPath(currentPathIndex), back), pathContainer.GetPath(currentPathIndex).Time, Random.Range(idleTimeMin, idleTimeMax));
        }

        Vector3[] GetWaypoints(PathContainer.Path path, bool back)
        {
            Vector3[] ret = new Vector3[path.Waypoints.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = !back ? path.Waypoints[i].position : path.Waypoints[ret.Length-1-i].position;

            return ret;

        }

        void Tween(Vector3[] waypoints, float time, float delay)
        {
            tween = gameObject.transform.DOPath(waypoints, time, PathType.CatmullRom, PathMode.Full3D).OnComplete(HandleOnPathComplete);
            tween.SetDelay(delay).SetEase(Ease.InOutFlash).SetSpeedBased();

            
        }

        List<Transform> GetAvailableTargets()
        {
            List<Transform> ret = new List<Transform>();

            // Get the ball
            if (Ball.Instance)
                ret.Add(Ball.Instance.transform);

            // Check for players
            PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
            foreach (PlayerController player in players)
                ret.Add(player.transform);

            return ret;
        }

        IEnumerator UpdateTarget()
        {
            while (true)
            {
                List<Transform> availableTargets = GetAvailableTargets();
                if(availableTargets.Count > 0)
                {
                    target = availableTargets[Random.Range(0, availableTargets.Count)];
                }

                yield return new WaitForSeconds(Random.Range(targetTimeMin, targetTimeMax));
            }
        }
    }

}
