using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    float originalFOV;
    [SerializeField]
    float zoomX1;
    [SerializeField]
    float zoomX2;

    int zoomLevel = 1;
    int minZoomLevel = 1;
    int maxZoomLevel = 3;
    
    void Start()
    {
        originalFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            zoomLevel += 1;
            zoomLevel = Mathf.Min(zoomLevel, maxZoomLevel);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            zoomLevel -= 1;
            zoomLevel = Mathf.Max(zoomLevel, minZoomLevel);
        }

        if (zoomLevel == 1)
        {
            Camera.main.fieldOfView = originalFOV;
        }
        if (zoomLevel == 2)
        {
            Camera.main.fieldOfView = originalFOV * zoomX1;
        }
        if (zoomLevel == 3)
        {
            Camera.main.fieldOfView = originalFOV * zoomX2;
        }
    }
}
