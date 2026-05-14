using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActualDataPanel
{
    public int level;
    public float maxHealth;
    public float curHealth;
    public float attack;
    public float defence;
    public float speed;
    public float criticalHitRate;
    public float criticalHitDamage;
    public float damageBonus;

    public float talentCooltimer;
    public bool burstEnergyAmple;
    
    public int bulletCount;

    public void LifeRecovery(float num)
    {
        curHealth = Mathf.Min(curHealth + num, maxHealth);
    }
}
