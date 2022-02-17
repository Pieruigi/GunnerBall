using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class ElectricGrenade : MonoBehaviour
    {
        [SerializeField]
        float range;

        int targetTeam;


        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Explode());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator Explode()
        {
            yield return new WaitForSeconds(1f);

            // Explosion range
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);

            // Freeze every player in the range belonging to the target team
            foreach(Collider collider in colliders)
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (!player) // Its not a player
                    continue;

                // Check the player team
                Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(player.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
                if(team == (Team)targetTeam)
                {
                    // Freeze player
                    player.SetFreezed();
                }
            }

        }

        public void SetTargetTeam(int team)
        {
            targetTeam = team;
        }
    }

}
