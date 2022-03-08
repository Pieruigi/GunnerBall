using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class BallStepParticlesController : MonoBehaviour
    {
        #region private fields
        ParticleSystem particles;
        GameObject owner;
        ParticleSystem ps;

        float groundDistance = 0.2f;
        #endregion


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {

            Ray ray = new Ray(owner.transform.position, Vector3.down);
            int mask = LayerMask.GetMask(new string[] { Layer.Ground });
            if (Physics.Raycast(ray, groundDistance, mask))
            {
                // Activate the particle
                if (!particles.isPlaying)
                    particles.Play();
            }
            else
            {
                // Deactivate the particle
                if (particles.isPlaying)
                    particles.Stop();
            }
        }

        #region public methods
        public void Init(GameObject particlesPrefab, GameObject owner)
        {
            // Create the particle system
            this.particles = GameObject.Instantiate(particlesPrefab).GetComponent<ParticleSystem>();
            // Set parent
            this.particles.transform.parent = owner.transform;
            // Place in position
            this.particles.transform.localPosition = Vector3.zero;

            this.owner = owner;

        }
        #endregion
    }

}
