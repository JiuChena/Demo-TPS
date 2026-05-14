using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageReceiver : DamageReceiver
{
    public override UnitActualDataPanel GetSelfDataPanel
    {
        get
        {
            return PlayerControlModule.Instance.GetCHActualDataPanel;
        }
        protected set
        {
            PlayerControlModule.Instance.GetCHActualDataPanel = value;
        }
    }
    
    public bool damagePush = false;
    public float damage = 100;

    private void Update()
    {
        if (damagePush)
        {
            PushDamage(damage);
            damagePush = false;
        }
    }

    public override void PushDamage(float damage)
    {
        GetSelfDataPanel.curHealth = Mathf.Max(GetSelfDataPanel.curHealth - damage, 0);

        if (GetSelfDataPanel.curHealth == 0)
        {
            PlayerControlModule.Instance.inputData.death = true;
        }
    }
}
