using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo/New CharacterType", fileName = "Assets/Configurations/ItemInfos/ItemCharacter/New CharacterType")]
public class ItemCharacterInfo : ItemInfoBase
{
    public override bool Buy(int amount)
    {
        if ((amount * value) <= DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money, "Money"))
        {
            if (!PlayerControlModule.Instance.levelDic.AddCH(itemName))
            {
                //通知相同角色只允许有一个
                return false;
            }
            DataCenter.Instance.storeData.SetStoreItemRemain(itemName, amount);
            DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Money, "Money", value * amount);
            return true;
        }
        else
        {
            PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Only one of the same character is allowed");
            return false;
        }
    }

    public override void Use()
    {
        
    }
}
