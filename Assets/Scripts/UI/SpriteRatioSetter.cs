using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca.UI
{
    public class SpriteRatioSetter : MonoBehaviour
    {
        [SerializeField]
        Vector2 ratioReference;

        float ratioValue;
        
        // Start is called before the first frame update
        void Start()
        {
            Adjust();
        }

        // Update is called once per frame
        void Update()
        {
            Adjust();
        }

        private void OnEnable()
        {
            if(SettingsManager.Instance)
                SettingsManager.Instance.OnResolutionChanged += HandleOnResolutionChanged;

        }

        private void OnDisable()
        {
            if (SettingsManager.Instance)
                SettingsManager.Instance.OnResolutionChanged -= HandleOnResolutionChanged;
        }

        void HandleOnResolutionChanged()
        {
            Adjust();
        }

        void Adjust()
        {
            ratioValue = (float)Screen.width / (float)Screen.height;
            float refRatioValue = ratioReference.x / ratioReference.y;
            Vector2 scale = Vector2.one;

            if (ratioValue > refRatioValue) // Horizontal stretch
            {

                scale.x = ratioValue / refRatioValue;
            }
            else // Vertical stretch
            {
                if(ratioValue < refRatioValue)
                {
                    scale.y = refRatioValue / ratioValue;
                }
                
            }

            transform.localScale = scale;
            
        }
    }

}
