using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class OptionManager : MonoBehaviour
    {
        public static OptionManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                Init();

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

//        [SerializeField]
        List<Option> optionList;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region private
        void Init()
        {
            optionList = new List<Option>();

            // Add resolution option
            optionList.Add(new Option(OptionType.Resolution));
        }
        #endregion
    }

}
