using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        Vector3 speed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(speed * Time.deltaTime, Space.Self);
        }
    }

}
