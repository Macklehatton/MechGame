using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerRangedAttack : MonoBehaviour
{
    public bool raycastTrue;
    public RaycastHit weaponRaycastHit;
    public float currentSpreadAngle;

    [SerializeField]
    private Transform weaponRaycastOrigin;
    [SerializeField]
    private WeaponStats equippedWeapon;
    [SerializeField]
    private int equippedWeaponID;
    [SerializeField]
    private WeaponList weaponList;
    [SerializeField]
    private AudioSource weaponAudio;
    
    private PlayerTargeting targeting;
    private TurretRotation turretRotation;

    private float comparisonMargin;

    private bool fireButtonUp;
    private float timeSinceFire;

    private void Start()
    {
        targeting = GetComponent<PlayerTargeting>();
        turretRotation = GetComponent<TurretRotation>();
        comparisonMargin = 0.001f;
        equippedWeapon = weaponList.weapons[equippedWeaponID];
        weaponAudio = GetComponent<AudioSource>();
        weaponAudio.clip = equippedWeapon.FireSound;
    }

    private void Update()
    {
        timeSinceFire += Time.deltaTime;

        GetWeaponRaycast();

        if (equippedWeapon.FireMode == FireMode.Continuous)
        {
            if (Input.GetButton("Fire1"))
            {
                if (timeSinceFire >= equippedWeapon.RateOfFire)
                {
                    timeSinceFire = 0.0f;
                    Attack();
                    turretRotation.ApplyKick(equippedWeapon.RecoilKick);
                    weaponAudio.PlayOneShot(equippedWeapon.FireSound);
                }
            }
            else
            {
                //currentSpreadAngle -= Time.deltaTime * equippedWeapon.RecoverySpeed;
            }
        }
        else if (equippedWeapon.FireMode == FireMode.Single)
        {
            if (Input.GetButtonDown("Fire1") && fireButtonUp == true)
            {
                if (timeSinceFire >= equippedWeapon.RateOfFire)
                {
                    timeSinceFire = 0.0f;
                    fireButtonUp = false;
                    Attack();
                    turretRotation.ApplyKick(equippedWeapon.RecoilKick);
                    weaponAudio.PlayOneShot(equippedWeapon.FireSound);
                }
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                fireButtonUp = true;
            }
            else
            {
                //currentSpreadAngle -= Time.deltaTime * equippedWeapon.RecoverySpeed;
            }
        }

        currentSpreadAngle -= Time.deltaTime * equippedWeapon.RecoverySpeed;
        if (currentSpreadAngle < equippedWeapon.InitialSpreadAngle - comparisonMargin)
        {
            currentSpreadAngle = equippedWeapon.InitialSpreadAngle;
        }
        if (currentSpreadAngle > equippedWeapon.MaxSpreadAngle + comparisonMargin)
        {
            currentSpreadAngle = equippedWeapon.MaxSpreadAngle;
        }
    }

    private void Attack()
    {
        currentSpreadAngle += equippedWeapon.RecoilSpread;

        if (targeting.currentTarget != null)
        {
            //Destroy(targeting.currentTarget.gameObject);
            //targeting.RefreshTargets();
        }
    }

    private void GetWeaponRaycast()
    {
        if (Physics.Raycast(weaponRaycastOrigin.position, weaponRaycastOrigin.forward, out weaponRaycastHit, equippedWeapon.MaxRange))
        {
            raycastTrue = true;
        }
        else
        {
            raycastTrue = false;
        }
    }
}
