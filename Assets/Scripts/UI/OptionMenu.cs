using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject[] menuList;

        int currentMenuId = 0;

        private void Awake()
        {
            HideAll();

            gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            currentMenuId = 0;
            menuList[currentMenuId].SetActive(true);
        }

        private void OnDisable()
        {
            HideAll();   
        }

        public void OpenMenu(int id)
        {
            currentMenuId = id;
            HideAll();
            menuList[id].SetActive(true);
        }

        
        #region private
        void HideAll()
        {
            foreach (GameObject g in menuList)
                g.SetActive(false);
        }
        #endregion
    }

}
