using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class MiscSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        OptionSelector nicknameOption;


        private void Awake()
        {
            nicknameOption.OnChange += delegate(int id) 
            {
                SettingsManager.Instance.SetHideNickname(id == 0 ? false : true);
            };
            
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
            if (!SettingsManager.Instance)
                return;

            // Init UI
            InitNicknameOption();
        }

        private void OnDisable()
        {
           
        }

     

        #region private



        void InitNicknameOption()
        {
            // Set label
            nicknameOption.SetLabel("In Game Names");

            // Set options
            nicknameOption.SetOptions(new List<string> { "Show", "Hide" });

            int currentOptionId = SettingsManager.Instance.HideNickname ? 1 : 0;
            nicknameOption.SetCurrentOptionId(currentOptionId);
        }

       
        #endregion
    }

}
