using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo/New PotionType", fileName = "Assets/Configurations/ItemInfos/ItemPotion/New PotionType")]
public class ItemPotionInfo : ItemInfoBase
{
    public PotionType potionType;

    public float numericalValue;
    public float buffTime;

    public override bool Buy(int amount)
    {
        if ((amount * value) <= DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money, "Money"))
        {
            DataCenter.Instance.bagDataDic.AddItemToBag(ItemType.Potion, itemName, amount);
            DataCenter.Instance.storeData.SetStoreItemRemain(itemName, amount);
            DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Money, "Money", value * amount);
            return true;
        }
        else
        {
            PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Not enough money");
            return false;
        }
    }

    public override void Use()
    {
        switch (potionType)
        {
            case PotionType.Health:
                PlayerControlModule.Instance.GetCHActualDataPanel.LifeRecovery(numericalValue);
                break;
            case PotionType.Attack:
                PlayerControlModule.Instance.GetCHBonusPanel.AddAttackBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.Defence:
                PlayerControlModule.Instance.GetCHBonusPanel.AddDefenceBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.Speed:
                PlayerControlModule.Instance.GetCHBonusPanel.AddSpeedBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.CriticalHitRate:
                PlayerControlModule.Instance.GetCHBonusPanel.AddCritRateBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.CriticalHitDamage:
                PlayerControlModule.Instance.GetCHBonusPanel.AddCritDamageBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.DamageBonus:
                PlayerControlModule.Instance.GetCHBonusPanel.AddDamageBuffToPlayer(numericalValue, buffTime);
                break;
            case PotionType.EnergyEfficiency:
                PlayerControlModule.Instance.GetCHBonusPanel.AddEnergyEfficiencyBuffToPlayer(numericalValue, buffTime);
                break;
        }
        
        DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Potion, this.name, 1);
    }
}


