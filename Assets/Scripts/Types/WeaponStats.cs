using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
[HideReferenceObjectPicker]
[ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = "Add", Expanded = true, ListElementLabelName = "Name")]
public class WeaponStats
{
    [SerializeField]
    public int ID;
    [SerializeField]
    public string Name;
    [SerializeField]
    [MultiLineProperty]
    public string Description;
    [SerializeField]
    public float KineticDamage;
    [SerializeField]
    public bool Explosive;
    [SerializeField]
    [ShowIf("Explosive")]
    public float ExplosiveDamage;
    [SerializeField]
    public float AP;
    [SerializeField]
    public float MaxRange;

    // Degrees per shot
    [SerializeField]
    public float RecoilSpread;
    [SerializeField]
    public float RecoilKick;
    [SerializeField]
    public float RateOfFire;

    // Degrees per second
    [SerializeField]
    public float RecoverySpeed;
    [SerializeField]
    public float InitialSpreadAngle;
    [SerializeField]
    public float MaxSpreadAngle;
    [SerializeField]
    public FireMode FireMode;

    [SerializeField]
    public AudioClip FireSound;

    //weapon effects as class
    //prefab
}
