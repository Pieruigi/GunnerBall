using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> popUpObjects;

        float showTime;

        private void Awake()
        {
            // Set the cursor visible 
            Cursor.lockState = CursorLockMode.None;

            ShowObjects(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(showTime > 0)
            {
                showTime -= Time.deltaTime;

                // Not yet
                if (showTime > 0)
                    return;


                // Show or hide button
                ShowObjects(true);

            }
        }

        private void OnEnable()
        {
            showTime = 1;
        }

        private void OnDisable()
        {
            showTime = 0;
            ShowObjects(false);
        }


        void ShowObjects(bool value)
        {
            foreach (GameObject o in popUpObjects)
            {
                o.SetActive(value);
            }
        }
    }

}
