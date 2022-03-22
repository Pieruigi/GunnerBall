using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerLable : MonoBehaviour
{
    [SerializeField]
    Transform target;
        
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
        
        Vector3 point = Camera.main.WorldToScreenPoint(target.position);
        Debug.Log("Point:" + point);
        RectTransform rt = transform as RectTransform;
        rt.anchoredPosition = point;

    }
}
