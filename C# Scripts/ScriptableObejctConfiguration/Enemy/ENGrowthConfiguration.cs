using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/CEData Growth/Enemy/New CHGrowthConfiguration", menuName = "Create New Config/Enemy/ENGrowthConfiguration")]
public class ENGrowthConfiguration : CESODataGrowth
{
    [Header("普通攻击每一级的成长度")] public float normalAttackGrowth;
    [Header("天赋技能每一级的成长度")] public float talentGrowth;
}
