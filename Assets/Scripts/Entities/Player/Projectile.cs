using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [HideInInspector] public Action_Shoot myPool;

    public abstract void Impact(Health.ArmorType armorType);
}
