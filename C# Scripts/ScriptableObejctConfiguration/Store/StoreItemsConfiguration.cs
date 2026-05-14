using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Store/New StoreItemsConfiguration", fileName = "Assets/Configurations/StoreItemsConfiguration/New StoreItemsConfiguration")]
public class StoreItemsConfiguration : ScriptableObject
{
    [Header("商店出售物品批次编号")] public string storeSerialNumber;
    [Header("可被出售的子弹信息以及数量")] public List<StoreItemInfo<ItemAmmunitionInfo>> canBeSoldAmmunitionInfo;
    [Header("可被出售的药剂信息以及数量")] public List<StoreItemInfo<ItemPotionInfo>> canBeSoldPotionInfo;
    [Header("可被出售的道具信息以及数量")] public List<StoreItemInfo<ItemPropInfo>> canBeSoldPropInfo;
    [Header("可被出售的芯片信息以及数量")] public List<StoreItemInfo<ItemChipInfo>> canBeSoldChipInfo;
    [Header("可被出售的角色信息以及数量")] public List<StoreItemInfo<ItemCharacterInfo>> canBeSoldCharacterInfo;
}
