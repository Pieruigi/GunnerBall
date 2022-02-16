using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class Barrier : MonoBehaviour
    {
        [SerializeField]
        float height;

        private void Awake()
        {
            // Put under the ground
            Vector3 pos = transform.position;
            pos.y -= height;
            transform.position = pos;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Move up
            transform.DOMoveY(transform.position.y + height, 0.25f, false).SetEase(Ease.OutBounce);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
