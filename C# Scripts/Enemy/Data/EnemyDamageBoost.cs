using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageBoost
{
    public float healthEnhance;
    public float attackEnhance;
    public float defenseEnhance;
    public float speedEnhance;
    public float criticalRateEnhance;
    public float criticalDamageEnhance;
    public float damageEnhance;

    public void BoostReset()
    {
        healthEnhance = 0;
        attackEnhance = 0;
        defenseEnhance = 0;
        speedEnhance = 0;
        criticalRateEnhance = 0;
        criticalDamageEnhance = 0;
        damageEnhance = 0;
    }
}
