using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CESODataBase : ScriptableObject
{
    [Header("基础数值区")]
    [Tooltip("生命值")] public float health = 1000f;
    [Tooltip("攻击力")] public float attack = 120f;
    [Tooltip("防御力")] public float defence = 450f;
    [Tooltip("移动速度")] public float moveSpeed = 3f;
    
    [Space(10)]
    
    [Header("提伤乘区")]
    [Tooltip("暴击率")] public float criticalHitRate = 0.25f;
    [Tooltip("暴击伤害")] public float criticalHitDamage = 1f;
    [Tooltip("伤害加成")] public float damageBonus = 0;
    
    [Space(10)]
    
    [Header("能值区")]
    [Tooltip("消耗蓄能值")] public float energyStorageValue = 2;
}
