using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Zoca
{
    public enum MatchState { None, Paused, Started, Goaled, Completed }

    /// <summary>
    /// This class implement match rules
    /// </summary>
    public class Match : MonoBehaviour, IOnEventCallback
    {

        public UnityAction OnStateChanged;
        //public UnityAction OnGoalScored;

        public static Match Instance { get; private set; }

        float timeElapsed;
        public float TimeElapsed
        {
            get { return timeElapsed; }
        }

        float stateTimestamp;
        public float StateTimestamp
        {
            get { return stateTimestamp; }
        }

        float matchLength;
        public float Length
        {
            get { return matchLength; }
        }

        int state = (int)MatchState.None;
        public int State
        {
            get { return state; }
        }

        int oldState = (int)MatchState.None;


        int blueTeamScore = 0, redTeamScore = 0;
        public int BlueTeamScore
        {
            get { return blueTeamScore; }
        }
        public int RedTeamScore
        {
            get { return redTeamScore; }
        }

        float targetTime;
        public float TargetTime
        {
            get { return targetTime; }
        }


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                // Load all custom prperties
                matchLength = (int)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchLength);
                timeElapsed = (float)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed);
                stateTimestamp = (float)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp);
                state = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState);
                oldState = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState);
                blueTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore);
                redTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore);
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

            //if(state == (int)MatchState.Paused)
            //{
            //    // Player can only look around
            //    playerController.MoveDisabled = true;
            //    playerController.ShootDisabled = true;
            //}
            UpdateState();
        }

        // Update is called once per frame
        void Update()
        {
            // All clients can execute this method
            PerformState();

            if(PhotonNetwork.IsMasterClient)
                CheckState();
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void Goal(Team team)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            // Save custom properties
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Goaled);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)state);
            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);

            if(team == Team.Blue)
            {
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore, (byte)(blueTeamScore+1));
            }
            else
            {
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore, (byte)(redTeamScore + 1));
            }

            // Write
            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

            // Send an event to all
            // Send event to the others
            RaiseStateChangedEvent();
        }
        
        
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            switch (eventCode)
            {
                case PhotonEvent.StateChanged:

                    state = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState);
                    oldState = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState);
                    stateTimestamp = (float)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp);

                    UpdateState();

                    break;

            }
        }

        /// <summary>
        /// Executed from all clients.
        /// This method is called by the RaiseStateChangeEvent() for each client in the 
        /// room.
        /// </summary>
        void UpdateState()
        {
            switch (state)
            {
                case (int)MatchState.Paused:
                    // Reset players and ball
                    if(Ball.Instance)
                        Ball.Instance.ResetBall();

                    PlayerController.LocalPlayer.GetComponent<PlayerController>().MoveDisabled = true;
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().ShootDisabled = true;
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().ResetPlayer();

                    // Set target time
                    targetTime = stateTimestamp + Constants.StartDelay;
                    break;
                case (int)MatchState.Started:
                    // To be sure get the time elapsed from the properties
                    timeElapsed = (float)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed);
                    
                    // Set player
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().MoveDisabled = false;
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().ShootDisabled = false;
                    break;
                case (int)MatchState.Goaled:

                    // Get data
                    redTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.RedTeamScore);
                    blueTeamScore = (byte)RoomCustomPropertyUtility.GetCurrentRoomCustomProperty(RoomCustomPropertyKey.BlueTeamScore);

                    PlayerController.LocalPlayer.GetComponent<PlayerController>().ShootDisabled = false;
                    // Set target time
                    targetTime = stateTimestamp + Constants.GoalDelay;
                    break;
                case (int)MatchState.Completed:
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().MoveDisabled = false;
                    PlayerController.LocalPlayer.GetComponent<PlayerController>().ShootDisabled = false;
                    break;
            }

            OnStateChanged?.Invoke();
        }

        /// <summary>
        /// All clients can execute this method
        /// </summary>
        void PerformState()
        {
            switch (state)
            {
                case (int)MatchState.Started:
                    // We update the time elapsed here rather than in the check state
                    // to let the other player to keep trace of this value, in 
                    // case of the master client would change.
                    timeElapsed += Time.deltaTime;
                    break;
            }
        }

        /// <summary>
        /// Only the master client check for a state change.
        /// The others have all the info to become master client themselves.
        /// </summary>
        void CheckState()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            switch (state)
            {
                case (int)MatchState.Paused:
                    // Every client must check the starting time and switch the game to the running state
                    if ((float)PhotonNetwork.Time > targetTime)
                    {

                        // Set state started
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Started);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)state);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);

                        if(oldState != (int)MatchState.None)
                        {
                            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, (float)timeElapsed);
                        }

                        // Write
                        RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

                        // Send event
                        RaiseStateChangedEvent();
                    }
                    
                    break;
                case (int)MatchState.Started:
                    
                    if(timeElapsed > matchLength)
                    {
                        // Set state completed
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Completed);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)state);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);

                        // Write
                        RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

                        RaiseStateChangedEvent();
                    }
                    break;

                case (int)MatchState.Goaled:
                    if ((float)PhotonNetwork.Time > targetTime)
                    {
                        // Set state completed
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, (byte)MatchState.Paused);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchOldState, (byte)state);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchStateTimestamp, (float)PhotonNetwork.Time);
                        RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, (float)timeElapsed);

                        // Write
                        RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

                        RaiseStateChangedEvent();
                    }
                    break;

                case (int)MatchState.Completed:
                    RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchTimeElapsed, (float)timeElapsed);

                    RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
                    RaiseStateChangedEvent();

                    break;

               
            }
        }

        void RaiseStateChangedEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEvent.StateChanged, null, raiseEventOptions, SendOptions.SendReliable);
        }

    }
}

