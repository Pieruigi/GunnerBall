using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class PowerUpShotTrail : MonoBehaviour
    {
        [SerializeField]
        float speed = 10f;

        [SerializeField]
        float lifeTime = 1;

        
        float dir = 0;
        Vector3 forward;
        float elapsed;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LifeTimer());

            // Play
            GetComponent<ParticleSystem>().Play();

            // Move
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += forward * speed * Time.deltaTime;
            elapsed += Time.deltaTime;

            if(elapsed > 0.25f && dir == 0)
            {
                dir = 0.2f;
                elapsed = 0;
            }

            if(elapsed > 0.25 && dir > 0)
            {
                dir = -0.4f;
                elapsed = 0;
            }

            if (elapsed > 0.25 && dir < 0)
            {
                dir = 0.2f;
                elapsed = -lifeTime;
            }

            if (dir != 0)
            {
                transform.position += dir * transform.right * speed * Time.deltaTime;
            }
        }

        IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(lifeTime);

            Destroy(gameObject);
        }

        private void Init()
        {
            forward = transform.forward;

            PlayerController player = GetComponentInParent<PlayerController>();
            if (player)
                forward = player.PlayerCamera.transform.forward;

            transform.parent = null;



        }

     
    }

}
