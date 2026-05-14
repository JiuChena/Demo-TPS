using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo/New AmmunitionType", fileName = "Assets/Configurations/ItemInfos/ItemAmmunition/New AmmunitionType")]
public class ItemAmmunitionInfo : ItemInfoBase
{
    public override bool Buy(int amount)
    {
        if ((amount * value) <= DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money, "Money"))
        {
            DataCenter.Instance.bagDataDic.AddItemToBag(ItemType.Ammunition, itemName, amount);
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
}
