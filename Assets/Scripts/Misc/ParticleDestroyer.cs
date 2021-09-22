using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class ParticleDestroyer : MonoBehaviour
    {
        [SerializeField]
        ParticleSystem particle;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!particle.isPlaying)
                Destroy(gameObject);
        }
    }

}
