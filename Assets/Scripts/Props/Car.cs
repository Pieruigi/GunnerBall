using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Zoca
{
    public class Car : MonoBehaviour
    {
        [SerializeField]
        float maxTime = 50;

        [SerializeField]
        float minTime = 25;

        [SerializeField]
        PlayableDirector director;

        [SerializeField]
        List<PlayableAsset> playables;

        float elapsed = 0;
        float targetTime;
        int currentPlayableId = -1;

        private void Awake()
        {
            targetTime = Random.Range(minTime, maxTime);
            director.stopped += OnDirectorStopped;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // If no director is playing check time
            if(currentPlayableId < 0)
            {
                elapsed += Time.deltaTime;
                if(elapsed > targetTime)
                {
                    // Choose a new director to play
                    currentPlayableId = Random.Range(0, playables.Count);
                  
                    director.playableAsset = playables[currentPlayableId];
                    director.Play();
                    Debug.Log("Director has started");
                }
            }
            
        }

        void OnDirectorStopped(PlayableDirector director)
        {
            Debug.Log("Director has stopped");
            currentPlayableId = -1;
            director.playableAsset = null;
            elapsed = 0;
            targetTime = Random.Range(minTime, maxTime);
        }
    }

}
