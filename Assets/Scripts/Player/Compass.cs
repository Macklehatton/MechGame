using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] Transform playerCamera;

    Vector3 startPosition;
    float pixelRatio;

    void Start()
    {
        startPosition = transform.localPosition;
        pixelRatio = GetComponent<Image>().rectTransform.rect.size.x  / 2 / 360.0f;
    }

    void Update()
    {
        Vector3 perp = Vector3.Cross(Vector3.forward, playerCamera.forward);
        float dir = Vector3.Dot(perp, Vector3.up);
        transform.localPosition = startPosition + (new Vector3(Vector3.Angle(playerCamera.forward, Vector3.forward) * Mathf.Sign(dir) * pixelRatio, 0, 0));
    }
}