using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageReceiver : MonoBehaviour
{
    public abstract UnitActualDataPanel GetSelfDataPanel { get; protected set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            BulletDriverModule bulletDriver = other.GetComponent<BulletDriverModule>();
            
            // float damage = GeneralDataHandler.DamageCalculator(bulletDriver.attackerDataPanel, GetSelfDataPanel);
            
            // PushDamage(damage);
        }
    }
    
    public abstract void PushDamage(float damage);

    //子弹上一定带有一个叫BulletDriverModule的模块,该模块包含攻击者的相关信息
}
