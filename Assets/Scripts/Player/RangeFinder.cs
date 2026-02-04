using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangeFinder : MonoBehaviour {

    [SerializeField]
    Text rangeText;
    
    private float distance;

    private void Start()
    {
        rangeText = GameObject.FindGameObjectWithTag("Rangefinder").GetComponent<Text>();
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2000);
        rangeText.text = ((int)hit.distance * 10).ToString();
    }
}
