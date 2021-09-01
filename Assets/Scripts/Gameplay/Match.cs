using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum MatchState { Starting, Running, Paused, Completed }

    /// <summary>
    /// This class implement match rules
    /// </summary>
    public class Match : MonoBehaviour, IOnEventCallback
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

        public int blueTeamScore = 0, redTeamScore = 0;
        float pauseTime;
        public float PauseTime
        {
            get { return pauseTime; }
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
                byte b = 0;
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<byte>(RoomCustomPropertyKey.BlueTeamScore, ref b);
                blueTeamScore = b;
                RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<byte>(RoomCustomPropertyKey.RedTeamScore, ref b);
                redTeamScore = b;

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

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        
        public void TeamScored(Team team)
        {
           
            if (team == Team.Blue)
            {
                blueTeamScore += 1;
            }
            else
            {
                redTeamScore += 1;
            }

            // Room custom properties synchronization is here
            SetNewState(MatchState.Paused);



            if (PhotonNetwork.IsMasterClient)
            {
                // Update room properties
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(team == Team.Blue ? RoomCustomPropertyKey.BlueTeamScore : RoomCustomPropertyKey.RedTeamScore, team == Team.Blue ? blueTeamScore : redTeamScore);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.PauseTime, (float)PhotonNetwork.Time);
                RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Paused);
                RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();

                // Send event to the others
                object[] content = new object[] { team };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(PhotonEvent.TeamScored, content, raiseEventOptions, SendOptions.SendReliable);
            }
            

        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            switch (eventCode)
            {
                case PhotonEvent.TeamScored:
                    if (PhotonNetwork.IsMasterClient)
                        return;
                    // Get the paused time
                    RoomCustomPropertyUtility.TryGetCurrentRoomCustomProperty<float>(RoomCustomPropertyKey.PauseTime, ref pauseTime);
                    TeamScored((Team)((object[])photonEvent.CustomData)[0]);
                    break;

            }
        }

        void SetNewState(MatchState newState)
        {
            Debug.LogFormat("Setting new state: {0}", newState);

            state = newState;

            if(newState == MatchState.Paused)
            {
                pauseTime = Time.time;

                // Reset the ball
                Ball.Instance.ResetBall();

                PlayerController.LocalPlayer.GetComponent<PlayerController>().MoveDisabled = true;
                PlayerController.LocalPlayer.GetComponent<PlayerController>().ShootDisabled = true;


                // Reset local player
                PlayerController.LocalPlayer.GetComponent<PlayerController>().ResetPlayer();

               
                

            }

            
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
                            
                            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Running);
                            //RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
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
                            

                            //RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Completed);
                            //RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
                        }
                    }
                    break;

                case MatchState.Completed:

                    break;

                case MatchState.Paused:
                    if(Time.time > pauseTime + Constants.PauseDelay)
                    {
                        state = MatchState.Running;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            RoomCustomPropertyUtility.AddOrUpdateCurrentRoomCustomProperty(RoomCustomPropertyKey.MatchState, MatchState.Running);
                            RoomCustomPropertyUtility.SynchronizeCurrentRoomCustomProperties();
                        }

                        PlayerController playerController = PlayerController.LocalPlayer.GetComponent<PlayerController>();
                        playerController.MoveDisabled = false;
                        playerController.ShootDisabled = false;
                    }
                    break;
            }
        }

        

    }
}

