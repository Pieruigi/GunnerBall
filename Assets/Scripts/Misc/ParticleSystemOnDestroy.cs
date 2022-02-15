using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca.Interfaces;

namespace Zoca
{
    public class ParticleSystemOnDestroy : MonoBehaviour
    {
        [SerializeField]
        float delay;

        bool started = false;

        private void Awake()
        {
            GetComponentInParent<IPickable>().OnPicked += HandleOnPicked;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!started)
                return;

            //if (!GetComponent<ParticleSystem>().isPlaying)
            //    Destroy(gameObject);
        }

        void HandleOnPicked(IPickable pickable, GameObject picker)
        {
            // Put the particle out of the object
            gameObject.transform.parent = null;

            // Play the particle
            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            yield return new WaitForSeconds(delay);

            started = true;
            GetComponent<ParticleSystem>().Play();
        }
    }

}
