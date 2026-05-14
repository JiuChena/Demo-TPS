using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreData
{
    private StoreLocalData storeLocalData = new StoreLocalData();

    public void LoadData()
    {
        storeLocalData = BinaryDataManager.Instance.LoadDataFromFile<StoreLocalData>("Data/Store/", "StoreData");
        storeLocalData = storeLocalData == null ? new StoreLocalData() : storeLocalData;
    }

    public void SaveData()
    {
        BinaryDataManager.Instance.SaveDataToFile("Data/Store/", "StoreData", storeLocalData);
    }

    public string GetStoreSerialNumber
    {
        get{ return storeLocalData.storeSerialNumber; }
    }

    public int GetStoreItemRemain(string itemName)
    {
        if (storeLocalData.dataDic.ContainsKey(itemName))
        {
            return storeLocalData.dataDic[itemName];
        }
        else
        {
            return 0;
        }
    }

    public void SetStoreItemRemain(string itemName, int count)
    {
        if (storeLocalData.dataDic.ContainsKey(itemName))
        {
            //如果该商品不限量或者数量不足，那么修改驳回
            if (storeLocalData.dataDic[itemName] == -1 || storeLocalData.dataDic[itemName] < count)
            {
                return;
            }
            else
            {
                storeLocalData.dataDic[itemName] -= count;
                SaveData();
            }
        }
    }

    public void UpdateData(StoreItemsConfiguration config)
    {
        storeLocalData.storeSerialNumber = config.storeSerialNumber;

        foreach (StoreItemInfo<ItemAmmunitionInfo> itemInfo in config.canBeSoldAmmunitionInfo) storeLocalData.dataDic[itemInfo.itemInfo.itemName] = itemInfo.itemPurchaseLimit;
        
        foreach (StoreItemInfo<ItemPotionInfo> itemInfo in config.canBeSoldPotionInfo) storeLocalData.dataDic[itemInfo.itemInfo.itemName] = itemInfo.itemPurchaseLimit;
        
        foreach (StoreItemInfo<ItemPropInfo> itemInfo in config.canBeSoldPropInfo) storeLocalData.dataDic[itemInfo.itemInfo.itemName] = itemInfo.itemPurchaseLimit;
        
        foreach (StoreItemInfo<ItemChipInfo> itemInfo in config.canBeSoldChipInfo) storeLocalData.dataDic[itemInfo.itemInfo.itemName] = itemInfo.itemPurchaseLimit;
        
        foreach (StoreItemInfo<ItemCharacterInfo> itemInfo in config.canBeSoldCharacterInfo) storeLocalData.dataDic[itemInfo.itemInfo.itemName] = itemInfo.itemPurchaseLimit;
        
        SaveData();
    }
}

[Serializable]
public class StoreLocalData
{
    public string storeSerialNumber;
    public SerializableDictionary<string, int> dataDic = new SerializableDictionary<string, int>();
}
