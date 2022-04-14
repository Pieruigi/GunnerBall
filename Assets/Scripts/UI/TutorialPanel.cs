using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class TutorialPanel : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        Transform buttonContainer;

        [SerializeField]
        Transform bodyContainer;

        List<GameObject> bodies;
        #endregion

        #region private methods
        private void Awake()
        {
           
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set listener to every button
            for (int i = 0; i < buttonContainer.childCount; i++)
            {
                int id = i;    
                buttonContainer.GetChild(id).GetComponent<Button>().onClick.AddListener(() => SetContent(id));
            }

            bodies = new List<GameObject>();
            for (int i = 0; i < bodyContainer.childCount; i++)
            {
                bodies.Add(bodyContainer.GetChild(i).gameObject);
            }

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            
            SetContent(0);
            
        }

        void SetContent(int id)
        {
            if (bodies == null)
                return;

            Debug.Log("Set content id:" + id);

            // Disable all content bodies
            HideContentAll();

            // Activate the first content
            bodies[id].SetActive(true);
        }
            
        void HideContentAll()
        {
            if (bodies == null)
                return;
            for (int i = 0; i < bodies.Count; i++)
            {
                
                bodies[i].SetActive(false);
            }
                
        }
        #endregion
    }

}
