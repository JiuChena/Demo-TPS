using System.Collections.Generic;
using UnityEngine;

public static class SkillEffectExecute
{
    public static void Execute(SkillEffect skillEffect, List<GameObject> targets, GameObject owner)
    {
        switch (skillEffect.effectType)
        {
            case SkillEffectType.InstantDamage:
                //找打击目标身上的状态组件中应用本次造成的伤害
                
                break;
            case SkillEffectType.InstantReply:
                //直接对owner使用状态恢复
                break;
            case SkillEffectType.BuffApplied:
                break;
        }
    }
}