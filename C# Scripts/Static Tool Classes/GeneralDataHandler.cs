using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralDataHandler
{
    #region 伤害计算器

    /// <summary>
    /// 等级减伤因数
    /// </summary>
    public static float LevelDamageReductionFactor = 0.02f;
    /// <summary>
    /// 等级最大减伤百分比
    /// </summary>
    public static float LevelMaxDamageReduction = 0.6f;
    /// <summary>
    /// 防御最大减伤百分比
    /// </summary>
    public static float defenceMaxDamageReduction = 0.6f;
    
    public static float DamageCalculator(float attackerSkillFactor, UnitActualDataPanel attacker, UnitActualDataPanel defender)
    {
        //伤害计算公式: a攻击力
        float levelDamageReduction = Mathf.Max(1 - LevelDamageReductionFactor * (defender.level - attacker.level), LevelMaxDamageReduction);

        float defenceDamageReduction = Mathf.Max(1 - defender.defence / attacker.attack, defenceMaxDamageReduction);

        bool criticalRate = Random.Range(0, 1000) / 1000f <= attacker.criticalHitRate;
        Debug.Log(criticalRate);
        Debug.Log("暴击率" + attacker.criticalHitRate);
        float finalDamage = criticalRate
            ? attacker.attack * attacker.criticalHitDamage * attacker.damageBonus * levelDamageReduction * defenceDamageReduction  //暴击
            : attacker.attack * attacker.damageBonus * levelDamageReduction * defenceDamageReduction;                              //没暴击
        
        return Mathf.Max(finalDamage, 1);
    }

    #endregion

    #region 实际角色面板计算处理

    public static UnitActualDataPanel CHActualPanelHandle(UnitActualDataPanel panel, CESODataBase config, CESODataGrowth growth, BonusPanel bonus, CHChipInfoData info, int CHLevel)
    {
        if (config != null)
        {
            panel.maxHealth = config.health * (growth.healthGrowth * CHLevel + 1) * (bonus.healthBonus + info.topChipInfo.GetChipNumericalValue(ChipType.HealthChip) + info.botChipInfo.GetChipNumericalValue(ChipType.HealthChip) + 1);
            panel.attack = config.attack * (growth.attackGrowth * CHLevel + 1) * (bonus.attackBonus + info.topChipInfo.GetChipNumericalValue(ChipType.AttackChip) + info.botChipInfo.GetChipNumericalValue(ChipType.AttackChip) + 1);
            panel.defence = config.defence * (growth.defenceGrowth * CHLevel + 1) * (bonus.defenceBonus + info.topChipInfo.GetChipNumericalValue(ChipType.DefenseChip) + info.botChipInfo.GetChipNumericalValue(ChipType.DefenseChip) + 1);
            panel.speed = config.moveSpeed * (bonus.speedBonus + info.topChipInfo.GetChipNumericalValue(ChipType.SpeedChip) + info.botChipInfo.GetChipNumericalValue(ChipType.SpeedChip) + 1);
            panel.criticalHitRate = config.criticalHitRate + (bonus.critRateBonus + info.topChipInfo.GetChipNumericalValue(ChipType.CriticalRateChip) + info.botChipInfo.GetChipNumericalValue(ChipType.CriticalRateChip));
            panel.criticalHitDamage = config.criticalHitDamage + (bonus.critDamageBonus + info.topChipInfo.GetChipNumericalValue(ChipType.CriticalDamageChip) + info.botChipInfo.GetChipNumericalValue(ChipType.CriticalDamageChip));
            panel.damageBonus = config.damageBonus + (bonus.damageBonus + info.topChipInfo.GetChipNumericalValue(ChipType.DamageBoostChip) + info.botChipInfo.GetChipNumericalValue(ChipType.DamageBoostChip));
        }

        return panel;
    }

    #endregion
}
