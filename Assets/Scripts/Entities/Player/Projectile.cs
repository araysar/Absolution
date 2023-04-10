using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [HideInInspector] public Character_Attack myPool;

    public abstract void Impact(Health.ArmorType armorType);
}
