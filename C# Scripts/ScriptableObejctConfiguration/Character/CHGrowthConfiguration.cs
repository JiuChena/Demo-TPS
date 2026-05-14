using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Assets/Configurations/CEData Growth/Character/New CHGrowthConfiguration", menuName = "Create New Config/Character/CHGrowthConfiguration")]
public class CHGrowthConfiguration : CESODataGrowth
{
     [Header("普通攻击每一级的成长度")] public float normalAttackGrowth;
     [Header("天赋技能每一级的成长度")] public float talentGrowth;
     [Header("爆发技能每一级的成长度")] public float burstGrowth;
     
     [Header("下一级升级角色所需要消耗的货币数额的因数")] public int moneyCostFactorForCHNextLevel = 40;
     [Header("下一级升级角色所需要消耗的经验道具数量的因数")] public int experienceItemCostFactorForCHNextLevel = 1;
     [Header("下一级升级技能所需要消耗的货币数额的因数")] public int moneyCostFactorForCSkillextLevel = 800;
     [Header("下一级升级技能所需要消耗的经验道具数量的因数")] public int experienceItemCostFactorForSkillNextLevel = 4;
}
