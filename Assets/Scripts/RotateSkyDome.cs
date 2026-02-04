using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkyDome : MonoBehaviour
{
    [SerializeField]
    float speed;

    void Update()
    {
        transform.Rotate(transform.up, speed * Time.deltaTime);        
    }
}
