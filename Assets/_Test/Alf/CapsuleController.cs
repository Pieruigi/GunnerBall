using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 5;

    [SerializeField]
    float mouseSens = 5;

    float pitch = 0;

    float pitchMax = 70;

    CharacterController cc;
    Transform cameraT;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject intCam = GetComponentInChildren<Camera>().gameObject;
        intCam.SetActive(false);

        if(Camera.main != null)
        {
            Destroy(intCam);
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = Vector3.up * 0.8f;
            Camera.main.transform.localRotation = Quaternion.identity;
            cameraT = Camera.main.transform;
        }
        else
        {
            intCam.SetActive(true);
            cameraT = intCam.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Look
        Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Yaw
        transform.Rotate(Vector3.up *lookInput.x * mouseSens * Time.deltaTime);

        // Pitch
        pitch += -lookInput.y * mouseSens * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -pitchMax, pitchMax);
        cameraT.localEulerAngles = Vector3.right * pitch;

        // Move
        Vector3 move = Vector3.zero;
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.x != 0)
            move += transform.right * input.x;

        if (input.y != 0)
            move += transform.forward * input.y;

        float yVel = 0;
        if (!cc.isGrounded)
            yVel = -9.82f * Time.deltaTime;

        move *= maxSpeed;

        move.y = yVel;

        cc.Move(move * Time.deltaTime);

    }
}
