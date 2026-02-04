using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [SerializeField]
    private float minOnIntensity;
    [SerializeField]
    private float maxOnIntensity;

    [SerializeField]
    private float minOffIntensity;
    [SerializeField]
    private float maxOffIntensity;

    [SerializeField]
    private float minOnTime;
    [SerializeField]
    private float maxOnTime;
    [SerializeField]
    private float minOffTime;
    [SerializeField]
    private float maxOffTime;

    private float transitionTime;

    private Light targetLight;

    private bool on;
    private float timeSinceTransition;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (on == true)
        {
            timeSinceTransition += Time.deltaTime;

            if (timeSinceTransition >= transitionTime)
            {
                on = false;
                targetLight.intensity = Random.Range(minOffIntensity, maxOffIntensity);
                transitionTime = Random.Range(minOffTime, maxOffTime);
                timeSinceTransition = 0;
            }
        }
        else if (on == false)
        {
            timeSinceTransition += Time.deltaTime;

            if (timeSinceTransition >= transitionTime)
            {
                on = true;
                targetLight.intensity = Random.Range(minOnIntensity, maxOnIntensity);
                transitionTime = Random.Range(minOnTime, maxOnTime);
                timeSinceTransition = 0;
            }

        }
    }
}
