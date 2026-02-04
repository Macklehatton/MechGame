using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationParameters : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float idleAfterMin;
    [SerializeField]
    private float idlefAfterMax;

    private Animator animator;
    private Vector3 lastPosition;

    private float timeIdle;
    private float idleAfterCurrent;
    private float lastSpeed;

    void Start()
    {
        lastPosition = Vector3.zero;
        animator = GetComponent<Animator>();
        timeIdle = 0.0f;
    }

    void Update()
    {
        float speed = (transform.position - lastPosition).magnitude;
        speed = speed / Time.deltaTime;

        if (Mathf.Approximately(lastSpeed, speed) == true)
        {
            timeIdle += Time.deltaTime;

            if (timeIdle > idleAfterCurrent)
            {
                animator.SetTrigger("Idle");
                timeIdle = 0.0f;
                idleAfterCurrent = Random.Range(idleAfterMin, idlefAfterMax);
            }
        }
        else
        {
            timeIdle = 0.0f;            
        }

        float speedRatio = Mathf.Max(0.0f, speed / maxSpeed);

        animator.SetFloat("ForwardSpeed", speedRatio);

        lastPosition = transform.position;
        lastSpeed = speed;
    }
}
