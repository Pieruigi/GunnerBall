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

        [SerializeField]
        GameObject fxPrefab; 

        int targetTeam;


        // Start is called before the first frame update
        void Start()
        {
            // If the local player doesn't belong to the target team return and the match is 
            // online we can return ( we don't take care of the other players )
            if (!PhotonNetwork.OfflineMode)
            {
                int team = (int)PlayerCustomPropertyUtility.GetPlayerCustomProperty(PlayerController.Local.photonView.Owner, PlayerCustomPropertyKey.TeamColor);

                if (team != targetTeam)
                    return;
            }

            StartCoroutine(Explode());
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator Explode()
        {
            GameObject fx = Instantiate(fxPrefab);
            fx.transform.position = transform.position;
            fx.transform.Translate(Vector3.forward * 10);
            Destroy(fx, 5);
            fx.GetComponent<ParticleSystem>().Play();

            yield return new WaitForSeconds(0.5f);

            // Explosion range
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);

            // Freeze every player in the range belonging to the target team
            foreach(Collider collider in colliders)
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (!player) // Its not a player
                    continue;

                // Only local player or AIs are going to be freezed by the grenade
                if(player.photonView.IsMine || PhotonNetwork.OfflineMode)
                {
                    // Check the player team
                    Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(player.photonView.Owner, PlayerCustomPropertyKey.TeamColor);
                    Debug.LogFormat("Player:{0}, Team:{1}", player, team);
                    if (team == (Team)targetTeam)
                    {
                        // Freeze player
                        player.SetFreezed();
                    }
                }
                
            }

        }

        public void SetTargetTeam(int team)
        {
            Debug.Log("Setting grenade target team:" + (Team)team);
            targetTeam = team;
        }
    }

}
