using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zoca.AI
{
    /// <summary>
    /// When playing games like football or shootball the whole team moves around the ball as one entity.
    /// Within the team each player can do micro movement depending on its specific behaviour.
    /// </summary>
    public class TeamHelper : MonoBehaviour
    {
        #region inspector fields
        [SerializeField]
        Team team = Team.Blue;

        [SerializeField]
        List<GameObject> formationPrefabs;

        [SerializeField]
        Transform middleField;

        [SerializeField]
        Transform ownedGoalLine;

        [SerializeField]
        Transform opponentGoalLine;

        [SerializeField]
        GameObject testFormation;
        #endregion

        #region properties
        public Transform MiddleField
        {
            get { return middleField; }
        }
        public Transform OpponentGoalLine
        {
            get { return opponentGoalLine; }
        }
        public Transform OwnedGoalLine
        {
            get { return ownedGoalLine; }
        }
        public int PlayersPerTeam
        {
            get { return playersPerTeam; }
        }
        public Team Team
        {
            get { return team; }
        }
        public IList<Transform> FormationHelpers
        {
            get { return formationHelpers.AsReadOnly(); }
        }
        #endregion

        #region private fields
        List<PlayerAI> players;
        int playersPerTeam = 1; // How many players per team
        GameObject formationObject;
        int formationIndex = 0;
        Transform ball;

        Transform closestFormationHelper;
        Transform lastClosestFormationHelper;
        List<Transform> formationHelpers = new List<Transform>();
        List<Transform> formationWaypoints = new List<Transform>();
        #endregion

        #region private
        private void Awake()
        {
            // Load formations
            
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        void LateUpdate()
        {
            //OrderHelpersByBallDistance();

            lastClosestFormationHelper = closestFormationHelper;
            closestFormationHelper = null;
        }

        void Init()
        {

            // Get the ball
            //if(GameObject.FindGameObjectWithTag(Tag.Ball))
            //    ball = GameObject.FindGameObjectWithTag(Tag.Ball).transform;

            // Get the number of ai playing in this team
            players = new List<PlayerAI>(GameObject.FindObjectsOfType<PlayerAI>()).FindAll(p => p.Team == team);
            playersPerTeam = players.Count;
            for(int i=0; i<playersPerTeam; i++)
            {
                players[i].SetWaypointIndex(i);
            }
           

            // Load all the formations matching the number of players
            //List<Transform> formationPrefabs = new List<object>(Resources.LoadAll(ResourceFolder.TeamFormations)).FindAll(f=>f.name.StartsWith(string.Format("{0}_", playersPerTeam)));
            foreach (GameObject f in formationPrefabs)
                Debug.LogFormat("[Formation {0} loaded.]", f.name);

            // Get the formation prefab index
            formationIndex = formationPrefabs.FindIndex(f => f.name.StartsWith(string.Format("{0}_", PhotonNetwork.CurrentRoom.MaxPlayers/2)));

            // Create the formation game object
            SetFormationObject(formationPrefabs[formationIndex]);

            //OrderHelpersByBallDistance();

         
        }

        void SetFormationObject(GameObject formationPrefab)
        {
            
            if(formationObject)
            {
                // Clear list
                formationHelpers.Clear();
                
                // Destroy old
                Destroy(formationObject);

            }

            // Create new object
            formationObject = GameObject.Instantiate(formationPrefab);
            formationObject.transform.position = Vector3.zero;
            formationObject.transform.rotation = Quaternion.identity;
            formationObject.transform.rotation = transform.rotation;

            // Set helpers and waypoints 
            for(int i=0; i<formationObject.transform.childCount; i++)
            {
                formationHelpers.Add(formationObject.transform.GetChild(i));
            }
        }

        /// <summary>
        /// Rearrange the list of helper in increasing ordered by the distance from each helper and the ball
        /// </summary>
        /*
        void OrderHelpersByBallDistance()
        {
            // Compute the distance between the ball and each helper in the helper list
            // The distance list is used to avoid recalculating the distance with all the helpers
            List<float> distances = new List<float>();
            List<Transform> tmpHelpers = new List<Transform>();
            for (int i = 0; i < formationHelpers.Count; i++)
            {
                // Compute distance between the ball and the helper i
                float dist = Vector3.SqrMagnitude(ball.transform.position - formationHelpers[i].transform.position);
                // Confront with the others
                int index = -1;
                for (int j = 0; j < distances.Count && index < 0; j++)
                {
                    if (dist < distances[j])
                    {
                        index = j;
                    }
                }
                if (index < 0)
                {
                    // It's the biggest
                    index = distances.Count;
                }

                // Insert the distance and the helper
                distances.Insert(index, dist);
                tmpHelpers.Insert(index, formationHelpers[i]);
            }

            //Debug.Log("Order-----------------------");
            //foreach (Transform d in tmpHelpers)
            //    Debug.Log(d);

            formationHelpers = tmpHelpers;

        }
        */
        #endregion

        #region public
        public Transform GetTheClosestFormationHelper()
        {
            if (ball == null)
            {
                if (!GameObject.FindGameObjectWithTag(Tag.Ball))
                    return null;
                ball = GameObject.FindGameObjectWithTag(Tag.Ball).transform;
            }
                

            if (!closestFormationHelper)
            {
                // Compute
                float min = 0;
                int index = -1;
                for(int i=0; i<formationHelpers.Count; i++)
                {
                    // Compute distance
                    float d = Vector3.SqrMagnitude(ball.position - formationHelpers[i].transform.position);
                    if(index < 0 || d < min)
                    {
                        index = i;
                        min = d;
                    }
                }
                // Store helper
                closestFormationHelper = formationHelpers[index];
            }

            

            if(closestFormationHelper != lastClosestFormationHelper)
            {
                // Update waypoints
                formationWaypoints.Clear();
                for(int i=0; i<closestFormationHelper.childCount; i++)
                {
                    formationWaypoints.Add(closestFormationHelper.GetChild(i));
                }
            }

            return closestFormationHelper;
        }
        #endregion

        #region debug

        #endregion
    }

}
