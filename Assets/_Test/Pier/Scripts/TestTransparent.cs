using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransparent : MonoBehaviour
{
    [SerializeField]
    Renderer rend;

    [SerializeField]
    Material transparentMaterial;

    float alpha = 1;

    // Start is called before the first frame update
    void Start()
    {
        Material mat = rend.material;
        
        //transparentMaterial.SetColor("_BaseColor", new Color(0, 1, 0, 0.5f));
        rend.material = transparentMaterial;
        
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= Time.deltaTime;
        rend.material.SetColor("_BaseColor", new Color(0, 1, 0, alpha));

    }
}
