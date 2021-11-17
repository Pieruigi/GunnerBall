using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class PlayerMaterialSetter : MonoBehaviour
    {
        [SerializeField]
        Material blueMaterial;

        [SerializeField]
        Material redMaterial;

        [SerializeField]
        int materialId;

        // Start is called before the first frame update
        void Start()
        {
            // Get renderer
            Renderer rend = GetComponent<Renderer>();
            // Get player owner
            Player owner = GetComponentInParent<PhotonView>().Owner;
            
            Debug.Log("OwnerActorNumber:" + GetComponentInParent<PhotonView>().OwnerActorNr);
            Debug.Log("Owner:" + GetComponentInParent<PhotonView>().Owner);
            // Get team
            Team team = (Team)PlayerCustomPropertyUtility.GetPlayerCustomProperty(owner, PlayerCustomPropertyKey.TeamColor);
            // Get the renderer material array
            Material[] mats = rend.materials;
            // Replace the given material
            mats[materialId] = team == Team.Blue ? blueMaterial : redMaterial;
            // Set the new array
            rend.materials = mats;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
