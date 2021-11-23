using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    /// <summary>
    /// Set the sprite color of the corresponding team in the local player hud
    /// </summary>
    public class TeamSpriteSetter : MonoBehaviour
    {
        [SerializeField]
        Sprite blueSprite;

        [SerializeField]
        Sprite redSprite;

        private void Awake()
        {
            // Get the image
            Image image = GetComponent<Image>();

            // Get the local player team
            Team team = (Team)PlayerCustomPropertyUtility.GetLocalPlayerCustomProperty(PlayerCustomPropertyKey.TeamColor);

            // Set sprite 
            image.sprite = team == Team.Blue ? blueSprite : redSprite;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
