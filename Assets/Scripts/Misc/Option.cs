using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum OptionType { Resolution, ScreenMode }

    [System.Serializable]
    public class Option
    {
        [SerializeField]
        List<object> valueList;
        public IList<object> ValueList
        {
            get { return valueList; }
        }

        [SerializeField]
        OptionType type;

        [SerializeField]
        int currentValueId = 0;
        public int CurrentValueId
        {
            get { return currentValueId; }
        }

        [SerializeField]
        int newValueId = -1;

        public Option(OptionType optionType)
        {
            type = optionType;

            valueList = new List<object>();
            switch (type)
            {
                case OptionType.Resolution:
                    InitResolutionData();
                    break;

                case OptionType.ScreenMode:

                    break;
            }
        }
        
        

        public void SetNewValueId(int valueId)
        {
            newValueId = valueId;
            
        }

        public void Save()
        {
            currentValueId = newValueId;
            newValueId = -1;

            switch (type)
            {

            }
        }

        public void Cancel()
        {
            newValueId = -1;
        }


        #region init_data
        void InitResolutionData()
        {
            for(int i=0; i<Screen.resolutions.Length; i++)
            {
                // Init list
                valueList.Add(Screen.resolutions[i]);

                // Set current
                if (Screen.resolutions[i].Equals(Screen.currentResolution))
                    currentValueId = i;
            }
        }


        #endregion
    }

}
