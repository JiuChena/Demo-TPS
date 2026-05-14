using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class EnemySkillDataTable
{
    [Tooltip("最近距离")] public float minDistance;
    
    [Header("普通攻击")] 
    [Tooltip("每次普通攻击释放的攻击次数")] public int attackCount = 1;
    [Tooltip("攻击最远距离")] public float attackMaxDistance;
    [Tooltip("技能特效")] public GameObject attackEffect;
    [Tooltip("普攻技能效果")] public List<SkillEffect> attackEffects;
    
    [Space(10)] 
    [Header("技能")] [Tooltip("技能冷却时间")] public float skillCoolTime;
    [Tooltip("技能最远距离")] public float skillMaxDistance;
    [Tooltip("技能特效")] public GameObject skillEffect;
    [Space(10)]
    [Tooltip("倍率依附")] public MultiplierAffixType multiplierAffix; 
    [Tooltip("技能效果")] public List<SkillEffect> skillEffects;
}