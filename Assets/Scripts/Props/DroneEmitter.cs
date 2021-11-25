using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class DroneEmitter : MonoBehaviour
    {
        [SerializeField]
        Renderer emitter;

        [SerializeField]
        int materialId;

        [SerializeField]
        float speed;

        private void Awake()
        {
            // Clone material
            Material[] mats = emitter.materials;
            mats[materialId] = new Material(mats[materialId]);
            emitter.materials = mats;

            // Set random color

            mats[materialId].SetColor("_EmissionColor", GetRandomColor());
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        Color GetRandomColor()
        {
            Color ret = new Color();
            ret.r = Random.Range(0f, 1f);
            ret.b = Random.Range(0f, 1f);
            ret.g = Random.Range(0f, 1f);

            ret.a = 1;
            return ret;
        }
    }

}
