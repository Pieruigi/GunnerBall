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

            
            //StartCoroutine(DoPath());
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += forward * speed * Time.deltaTime;
            elapsed += Time.deltaTime;

    
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

        IEnumerator DoPath()
        {
            yield return new WaitForSeconds(lifeTime * 0.2f);

            dir = .2f;

            yield return new WaitForSeconds(lifeTime * 0.2f);

            dir = -.4f;

            yield return new WaitForSeconds(lifeTime * 0.2f);

            dir = .2f;

            yield return new WaitForSeconds(lifeTime * 0.2f);

            dir = 0;
        }
     
    }

}
