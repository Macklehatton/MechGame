using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurretRotation : MonoBehaviour
{
    [SerializeField]
    float yRotationSpeed;
    [SerializeField]
    float xRotationSpeed;

    [SerializeField]
    private GameObject turret;
    [SerializeField]
    private GameObject barrel;
    [SerializeField]
    private GameObject forwardReference;
    [SerializeField]
    private Transform cameraPivot;

    [SerializeField]
    private float turretSmoothRate;
    [SerializeField]
    private float recencyWeight;
    [SerializeField]
    private float xMin;
    [SerializeField]
    private float xMax;
    [SerializeField]
    private float yMin;
    [SerializeField]
    private float yMax;

    private PlayerTargeting targeting;
    private Vector3 originalTurretRotation;
    private Vector3 originalBarrelRotation;    
    private Vector3 currentVelocity;

    private Vector3 smoothedTargetRotation;

    private List<Vector3> pastRotations;

    private void Start()
    {
        cameraPivot = GetComponent<PlayerSetup>().cameraObject.transform;
        targeting = GetComponent<PlayerTargeting>();
        originalTurretRotation = turret.transform.eulerAngles;
        originalBarrelRotation = barrel.transform.eulerAngles;

        pastRotations = new List<Vector3>();
        ZeroOutPastRotations();
        
    }
    
    private void Update()
    {
        RotateTurretY();
        RotateBarrelX();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
    }

    public void ApplyKick(float kick)
    {
        float xRotation = barrel.transform.eulerAngles.x - kick;
        barrel.transform.eulerAngles = new Vector3(xRotation, barrel.transform.eulerAngles.y, barrel.transform.eulerAngles.z);
    }

    private void ZeroOutPastRotations()
    {
        for (int i = 0; i < 50; i++)
        {
            pastRotations.Add(Vector3.zero);
        }
    }

    private void RotateBarrelX()
    {
        float targetX = cameraPivot.eulerAngles.x;
        float y = originalBarrelRotation.y;
        float z = originalBarrelRotation.z;

        Vector3 targetEuler = new Vector3(transform.eulerAngles.x, turret.transform.eulerAngles.y, turret.transform.eulerAngles.z);
        targetEuler = Quaternion.Euler(targetEuler) * Vector3.forward;

        float angleDifference = Vector3.SignedAngle(turret.transform.forward, targetEuler, turret.transform.right);

        Quaternion targetRotation;
        targetRotation = Quaternion.LookRotation((targeting.targetTrackingPosition - turret.transform.position).normalized, turret.transform.up);
        targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x, y, z);

        if (angleDifference > xMax)
        {
            targetX = xMax;
        }
        else if (angleDifference < xMin)
        {
            targetX = xMin;
        }

        Quaternion previousRotation = barrel.transform.rotation;
        barrel.transform.localEulerAngles = targetRotation.eulerAngles;
        Quaternion targetGlobalRotation = barrel.transform.rotation;

        barrel.transform.rotation = previousRotation;

        //Quaternion targetRotation = Quaternion.Euler(new Vector3(targetX, y, z));
        barrel.transform.rotation = Quaternion.RotateTowards(barrel.transform.rotation, targetGlobalRotation, xRotationSpeed * Time.deltaTime);

        //barrel.transform.localEulerAngles = new Vector3(targetX, y, z);
    }

    private void RotateTurretY()
    {
        float turretX = originalTurretRotation.x;
        float targetTurretY = cameraPivot.eulerAngles.y;
        float turretZ = originalTurretRotation.z;

        float angleDifference = Vector3.SignedAngle(forwardReference.transform.forward, transform.forward, turret.transform.up);

        Quaternion targetRotation;

        targetRotation = Quaternion.LookRotation((targeting.targetTrackingPosition - turret.transform.position).normalized, turret.transform.up);
        targetRotation.eulerAngles = new Vector3(turretX, targetRotation.eulerAngles.y, turretZ);

        //smoothedTargetRotation = ExpLerp(turret.transform.eulerAngles, targetRotation.eulerAngles, turretSmoothRate);

        //smoothedTargetRotation = Vector3.SmoothDamp(smoothedTargetRotation,

        //if (angleDifference > yMax)
        //{
        //    targetTurretY = yMax;
        //}
        //else if (angleDifference < yMin)
        //{
        //    targetTurretY = yMin;
        //}

        //smoothedTargetRotation = RecencySmoothedRotation(targetRotation.eulerAngles);
        //pastRotations.Insert(0, smoothedTargetRotation);
        //pastRotations.RemoveAt(pastRotations.Count - 1);

        //targetRotation = Quaternion.Euler(smoothedTargetRotation);
        //Quaternion targetRotation = Quaternion.Euler(new Vector3(turretX, targetTurretY, turretZ));

        //turret.transform.rotation = targetRotation;

        turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, targetRotation, yRotationSpeed * Time.deltaTime);
    }
    
    private float WrapAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float angle = Vector3.Angle(vectorA, vectorB);
        Vector3 cross = Vector3.Cross(vectorA, vectorB);
        if (cross.y < 0)
        {
            angle = -angle;
        }
        return angle;
    }

    private Vector3 RecencySmoothedRotation(Vector3 targetRotation)
    {
        Vector3 result = targetRotation * pastRotations.Count * 2;
        for (int i = 0; i < pastRotations.Count; i++)
        {            
            result = (result + pastRotations[i] * (((pastRotations.Count / (i + 1)) * recencyWeight) / pastRotations.Count)) / 2.0f;
        }
        return result.normalized;
    }

    private Vector3 ExpLerp(Vector3 value, Vector3 target, float rate)
    {
        //return Vector3.Lerp(value, target, Mathf.Pow(-turretSmoothRate * Time.deltaTime, 2));        
        return Vector3.SmoothDamp(value, target, ref currentVelocity, turretSmoothRate);
    }
}
