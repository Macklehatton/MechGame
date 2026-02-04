using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float minVertical;
    [SerializeField]
    private float maxVertical;

    Vector2 rotation;

    private void Start()
    {
        rotation = new Vector2(0.0f, transform.localRotation.eulerAngles.y);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        rotation.y += Input.GetAxisRaw("Mouse X") * speed;
        rotation.x -= Input.GetAxisRaw("Mouse Y") * speed;

        if (rotation.x < minVertical)
        {
            rotation.x = minVertical;
        }
        else if (rotation.x > maxVertical)
        {
            rotation.x = maxVertical;
        }
        
        transform.localEulerAngles = rotation;
    }
}
