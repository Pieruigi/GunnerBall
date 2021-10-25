using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> panels;
        
        [SerializeField]
        GameObject defaultPanel;

        bool opened = false;
        public bool Opened
        {
            get { return opened; }
        }

        #region private
        private void Awake()
        {
            CloseAll();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void CloseAll()
        {
            foreach (GameObject panel in panels)
                panel.SetActive(false);
        }
        #endregion

        #region public
        public void Resume()
        {
            if (!opened)
                return;

            UIManager.Instance.CloseGameMenuUI();
        }

        public void Open()
        {
            if (opened)
                return;
            opened = true;
            CloseAll();
            defaultPanel.SetActive(true);
        }

        public void Close()
        {
            if (!opened)
                return;
            opened = false;
            CloseAll();
           
        }
        #endregion
    }

}
