using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    public Transform target;
    [SerializeField]
    private Vector3 offset;

    void Update()
    {
        transform.position = target.transform.position + offset;
    }
}
