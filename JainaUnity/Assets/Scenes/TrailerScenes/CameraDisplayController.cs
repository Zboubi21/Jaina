using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDisplayController : MonoBehaviour
{
    public KeyCode input;
    Camera cam;

    bool toggleOn;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        if(cam != null)
        {
            if (Input.GetKeyDown(input) && !toggleOn)
            {
                cam.enabled = true;
                toggleOn = true;
            }
            else if(Input.GetKeyDown(input) && toggleOn)
            {
                cam.enabled = false;
                toggleOn = false;
            }
        }
    }
}
