using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField]
    public Transform cameraTarget;

    public GameObject cameraObject;
    public GameObject canvasObject;

    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    private GameObject canvasPrefab;

    private void Awake()
    {        
        if (canvasObject == null)
        {
            canvasObject = Instantiate(canvasPrefab);
        }
        if (cameraObject == null)
        {
            cameraObject = Instantiate(cameraPrefab, transform.position, transform.rotation);
            cameraObject.GetComponent<CameraFollow>().target = cameraTarget;            
        }
    }
}
