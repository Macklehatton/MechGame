using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTarget
{
    public GameObject gameObject;
    public Transform transform;

    public CombatTarget(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
    }
}
