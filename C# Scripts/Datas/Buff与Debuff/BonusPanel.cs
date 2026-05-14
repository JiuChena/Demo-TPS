using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPanel
{
    public float healthBonus = 0;
    public float attackBonus = 0;
    public float defenceBonus = 0;
    public float speedBonus = 0;
    public float critRateBonus = 0;
    public float critDamageBonus = 0;
    public float damageBonus = 0;
    public float energyEfficiencyBonus = 0;

    public void ResetBonus()
    {
        healthBonus = 0;
        attackBonus = 0;
        defenceBonus = 0;
        speedBonus = 0;
        critRateBonus = 0;
        critDamageBonus = 0;
        damageBonus = 0;
        energyEfficiencyBonus = 0;
    }
    
    private BonusInfo tempBonusInfo;

    public void OnUpdate()
    {
        for (int i = 0; i < attackBonusQueue.Count; i++)
        {
            tempBonusInfo = attackBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                attackBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.Attack);
                Debug.Log("Remove Attack Buff");
            }
            else attackBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < defenceBonusQueue.Count; i++)
        {
            tempBonusInfo = defenceBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                defenceBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.Defence);
            }
            else defenceBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < speedBonusQueue.Count; i++)
        {
            tempBonusInfo = speedBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                speedBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.Speed);
            }
            else speedBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < critRateBonusQueue.Count; i++)
        {
            tempBonusInfo = critRateBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                critRateBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.CriticalHitRate);
            }
            else critRateBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < critDamageBonusQueue.Count; i++)
        {
            tempBonusInfo = critDamageBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                critDamageBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.CriticalHitDamage);
            }
            else critDamageBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < damageBonusQueue.Count; i++)
        {
            tempBonusInfo = damageBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                damageBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.Damage);
            }
            else damageBonusQueue.Enqueue(tempBonusInfo);
        }

        for (int i = 0; i < energyEfficiencyBonusQueue.Count; i++)
        {
            tempBonusInfo = energyEfficiencyBonusQueue.Dequeue();
            tempBonusInfo.remainTime -= Time.deltaTime;

            if (tempBonusInfo.remainTime <= 0)
            {
                energyEfficiencyBonus -= tempBonusInfo.bonus;
                PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").RemoveBonus(BonusType.EnergyEfficiency);
            }
            else energyEfficiencyBonusQueue.Enqueue(tempBonusInfo);
        }
    }
    
    private Queue<BonusInfo> attackBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> defenceBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> speedBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> critDamageBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> critRateBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> damageBonusQueue = new Queue<BonusInfo>();
    private Queue<BonusInfo> energyEfficiencyBonusQueue = new Queue<BonusInfo>();

    public void AddAttackBuffToPlayer(float num, float time)
    {
        attackBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.Attack);
        attackBonus += num;
    }

    public void AddDefenceBuffToPlayer(float num, float time)
    {
        defenceBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.Defence);
        defenceBonus += num;
    }

    public void AddSpeedBuffToPlayer(float num, float time)
    {
        speedBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.Speed);
        speedBonus += num;
    }

    public void AddCritRateBuffToPlayer(float num, float time)
    {
        critRateBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.CriticalHitRate);
        critRateBonus += num;
    }
    
    public void AddCritDamageBuffToPlayer(float num, float time)
    {
        critDamageBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.CriticalHitDamage);
        critDamageBonus += num;
    }

    public void AddDamageBuffToPlayer(float num, float time)
    {
        damageBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.Damage);
        damageBonus += num;
    }

    public void AddEnergyEfficiencyBuffToPlayer(float num, float time)
    {
        energyEfficiencyBonusQueue.Enqueue(new BonusInfo(num, time));
        PanelManager.Instance.GetPanel<SkillPanel>("Skill Panel").AddBonus(BonusType.EnergyEfficiency);
        energyEfficiencyBonus += num;
    }
}

public class BonusInfo
{
    public float bonus;
    public float remainTime;

    public BonusInfo(float bonus, float remainTime)
    {
        this.bonus = bonus;
        this.remainTime = remainTime;
    }
}
