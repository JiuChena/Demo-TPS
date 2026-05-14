using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo/New PropType", fileName = "Assets/Configurations/ItemInfos/ItemProp/New PropType")]
public class ItemPropInfo : ItemInfoBase
{
    public override void Use()
    {
        //打开角色面板使用
        Debug.Log("打开角色面板");
    }

    public override bool Buy(int amount)
    {
        //打开商城页面，锁定到材料购买
        if ((amount * value) <= DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money, "Money"))
        {
            DataCenter.Instance.bagDataDic.AddItemToBag(ItemType.Prop, itemName, amount);
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
