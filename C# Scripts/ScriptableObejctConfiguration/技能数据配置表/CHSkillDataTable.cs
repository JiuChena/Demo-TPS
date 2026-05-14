using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CHSkillDataTable
{
    [Header("普通攻击")] 
    [Tooltip("每次普通攻击释放的攻击次数")] public int attackCount = 1;
    [Tooltip("技能特效")] public GameObject attackSkillEffect;
    [Tooltip("普攻技能效果")] public List<SkillEffect> attackSkillEffects;
    [Space(10)]
    [Header("天赋技能")]
    [Tooltip("天赋技能覆盖区域配置")] public SkillAreaConfig talentAreaConfig;
    [Tooltip("技能特效")] public GameObject talentSkillEffect;
    [Space(10)]
    [Tooltip("倍率依附")] public MultiplierAffixType talentMultiplierAffix; 
    [Tooltip("天赋技能效果")] public List<SkillEffect> talentSkillEffects;
    
    [Space(10)]
    [Header("爆发技能")]
    [Tooltip("爆发技能覆盖区域配置")] public SkillAreaConfig burstAreaConfig;
    [Tooltip("技能特效")] public GameObject burstSkillEffect;
    [Space(10)]
    [Tooltip("倍率依附")] public MultiplierAffixType burstMultiplierAffix; 
    [Tooltip("天赋技能效果")] public List<SkillEffect> burstSkillEffects;
}



//基础数值 + 倍率依附（attack）*（技能倍率 + 成长因数 * 技能等级） = 伤害值



public enum BuffType
{
    
}

