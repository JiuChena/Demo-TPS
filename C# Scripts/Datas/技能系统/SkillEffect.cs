using System;
using UnityEngine;

[Serializable]
public class SkillEffect
{
    [Tooltip("技能效果Type")] public SkillEffectType effectType;
    [Tooltip("触发模式")] public SkillEffectTriggerMode triggerMode;
    [Tooltip("基础数值")] public float baseValue;
    [Tooltip("技能倍率")] public float magnification;
    [Tooltip("成长因数")] public float growthFactor;
    [Tooltip("施法者特效")] public GameObject casterEffectPrefab;
    [Tooltip("命中目标效果")] public SerializableDictionary<LayerMask, SkillHitEffect> skillHitEffects;
}