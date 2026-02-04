using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingUI : MonoBehaviour
{
    [SerializeField]
    private float targetBracketThickness;
    [SerializeField]
    private float bracketLengthFactor;
    [SerializeField]
    private Transform weaponTransform;
    [SerializeField]
    private float crosshairLength;
    [SerializeField]
    private float crosshairWidth;
    [SerializeField]
    private float crosshairSeparation;
    [SerializeField]
    private float crosshairRange;
    [SerializeField]
    private float crosshairSmoothRate;

    private PlayerTargeting targeting;
    private PlayerRangedAttack rangedAttack;
    private TargetBrackets targetBrackets;
    private WeaponCrosshair weaponCrosshair;

    private Vector3 smoothedCrosshairPosition;
    private Vector3 currentVelocity;

    private void Start()
    {
        targeting = GetComponent<PlayerTargeting>();
        rangedAttack = GetComponent<PlayerRangedAttack>();

        targetBrackets = new TargetBrackets(targetBracketThickness);
        weaponCrosshair = new WeaponCrosshair(weaponTransform, crosshairWidth);
        smoothedCrosshairPosition = Vector3.zero;
    }

    private void Update()
    {
        if (targeting.currentTarget != null)
        {
            targetBrackets.SetBounds(targeting.currentTarget, bracketLengthFactor);
            targetBrackets.DrawLines();
        }
        else
        {
            targetBrackets.SetActive(false);
        }

        if (rangedAttack.raycastTrue == true)
        {
            //smoothedCrosshairPosition = ExpLerp(smoothedCrosshairPosition, rangedAttack.weaponRaycastHit.point, crosshairSmoothRate);
            crosshairSeparation = CalculateCrosshairSeparation(rangedAttack.currentSpreadAngle);
            weaponCrosshair.UpdateCrosshair(crosshairLength, crosshairWidth, crosshairSeparation, rangedAttack.weaponRaycastHit.point);
        }
        else
        {
            if (targeting.currentTarget != null)
            {
                weaponCrosshair.DrawSetRange(crosshairLength, crosshairWidth, crosshairSeparation, Vector3.Distance(weaponTransform.position, targeting.currentTarget.transform.position));
            }
            else
            {
                weaponCrosshair.DrawSetRange(crosshairLength, crosshairWidth, crosshairSeparation, 1000.0f);
            }
            //weaponCrosshair.DrawCentered(crosshairLength, crosshairWidth, crosshairSeparation);
        }
    }

    private float CalculateCrosshairSeparation(float angle)
    {
        float angleScreenPercentage = angle / Camera.main.fieldOfView;
        float pixelRadius = angleScreenPercentage * Screen.height;
        return pixelRadius;
    }

    private Vector3 ExpLerp(Vector3 value, Vector3 target, float rate)
    {
        //return Vector3.Slerp(value, target, Mathf.Pow(-crosshairSmoothRate * Time.deltaTime, 2));        
        return Vector3.SmoothDamp(value, target, ref currentVelocity, Mathf.Pow(-crosshairSmoothRate * Time.deltaTime, 2));

    }
}
