using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum MatchState { Starting, Running, Completed }

    /// <summary>
    /// This class implement match rules
    /// </summary>
    public class Match : MonoBehaviour
    {

        public static Match Instance { get; private set; }

        float elapsed;
        public float TimeElapsed
        {
            get { return elapsed; }
        }

        float startTime;
        float matchLength;

        MatchState state = MatchState.Starting;
        public MatchState State
        {
            get { return state; }
        }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                // We keep trace of the elapsed time because we want to stop the time match for example
                // when a player gets a score.
                elapsed = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchElapsed];
                startTime = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.StartTime];
                matchLength = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchLength];
                state = (MatchState)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState];
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CheckState();
        }


        
        


        void CheckState()
        {
            switch (state)
            {
                case MatchState.Starting:
                    // Every client must check the starting time and switch the game to the running state
                    if (startTime + Constants.StartDelay > PhotonNetwork.Time)
                    {
                        state = MatchState.Running;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            // Update the room property
                            PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Running;
                        }
                    }
                    
                    break;
                case MatchState.Running:
                    elapsed += Time.deltaTime;
                    if(elapsed > matchLength)
                    {
                        // Game is completed
                        state = MatchState.Completed;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            // Update the state property
                            PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Completed;
                        }
                    }
                    break;

                case MatchState.Completed:

                    break;
            }
        }
    }
}

