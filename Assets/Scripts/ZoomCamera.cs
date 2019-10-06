using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float fieldOfView = 60f;
    public float maxZoomFOV = 20f;


    private bool wheelZooming = false;
    private Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();

    }

    void Start()
    {

    }

    private void Update()
    {
        WheelZoomInAndOut();
        Zoom();
    }

    private void WheelZoomInAndOut()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            wheelZooming = true;
        }

        if (thisCamera.fieldOfView <= maxZoomFOV && scroll > 0)
        {
            thisCamera.fieldOfView = maxZoomFOV;
        }
        else if (thisCamera.fieldOfView >= fieldOfView && scroll < 0)
        {

        }
        else
        {
            thisCamera.fieldOfView -= scroll * zoomSpeed;
        }
    }

    private void Zoom()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            wheelZooming = false;
        }

        if (wheelZooming == false)
        {
            if (Input.GetButton("Fire2"))
            {
                thisCamera.fieldOfView = maxZoomFOV;
            }
            else
            {
                thisCamera.fieldOfView = fieldOfView;
            }
        }

    }

    void LateUpdate()
    {

    }
}