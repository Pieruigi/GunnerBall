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

        float matchElapsed;
        public float TimeElapsed
        {
            get { return matchElapsed; }
        }

        float startTime;
        public float StartTime
        {
            get { return startTime; }
        }

        float matchLength;
        public float Length
        {
            get { return matchLength; }
        }

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
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<float>(RoomCustomPropertyKey.MatchLength, ref matchLength);
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<float>(RoomCustomPropertyKey.MatchElapsed, ref matchElapsed);
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<float>(RoomCustomPropertyKey.StartTime, ref startTime);
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<MatchState>(RoomCustomPropertyKey.MatchState, ref state);
                

                //matchLength = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchLength];
                //matchElapsed = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchElapsed];
                //startTime = (float)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.StartTime];
                //state = (MatchState)PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState];
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Get the local player 
            PlayerController playerController = PlayerController.LocalPlayer.GetComponent<PlayerController>();

            if(state == MatchState.Starting)
            {
                // Player can only look around
                playerController.MoveDisabled = true;
                playerController.ShootDisabled = true;
            }
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
                    if ((float)PhotonNetwork.Time > startTime + Constants.StartDelay)
                    {
                        state = MatchState.Running;

                        // Update the player controller
                        PlayerController playerController = PlayerController.LocalPlayer.GetComponent<PlayerController>();
                        playerController.MoveDisabled = false;
                        playerController.ShootDisabled = false;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            // Update the room property
                            //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Running;
                            //PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Running);
                            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
                        }
                    }
                    
                    break;
                case MatchState.Running:
                    matchElapsed += Time.deltaTime;
                    if(matchElapsed > matchLength)
                    {
                        // Game is completed
                        state = MatchState.Completed;

                        // Update the player controller
                        PlayerController playerController = PlayerController.LocalPlayer.GetComponent<PlayerController>();
                        playerController.ShootDisabled = false;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            // Update the state property
                            //PhotonNetwork.CurrentRoom.CustomProperties[RoomCustomPropertyKey.MatchState] = MatchState.Completed;
                            //PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Completed);
                            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
                        }
                    }
                    break;

                case MatchState.Completed:

                    break;
            }
        }
    }
}

