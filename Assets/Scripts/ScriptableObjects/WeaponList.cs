using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(menuName = "Game/Weapon List")]
public class WeaponList : SerializedScriptableObject
{    
    public List<WeaponStats> weapons;

    // On create
    private void Awake()
    {
        weapons = new List<WeaponStats>();
    }

    private void Add()
    {
        weapons.Add(new WeaponStats());
    }
}
