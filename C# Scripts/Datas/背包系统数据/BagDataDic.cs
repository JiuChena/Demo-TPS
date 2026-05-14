using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagDataDic : IEnumerable<KeyValuePair<ItemType, SerializableDictionary<string, int>>>
{
    private SerializableDictionary<ItemType, SerializableDictionary<string, int>> bagDataDic = new SerializableDictionary<ItemType, SerializableDictionary<string, int>>();

    /// <summary>
    /// 向背包中添加新物品
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemID"></param>
    /// <param name="amount"></param>
    public void AddItemToBag(ItemType itemType, string itemID, int amount)
    {
        if (!bagDataDic.ContainsKey(itemType))
        {
            bagDataDic.Add(itemType, new SerializableDictionary<string, int>(){ [itemID] = amount });
        }
        else
        {
            if (!bagDataDic[itemType].ContainsKey(itemID))
            {
                bagDataDic[itemType].Add(itemID, amount);
            }
            else
            {
                bagDataDic[itemType][itemID] += amount;
            }
        }
        
        SaveBagDataDic();
    }

    /// <summary>
    /// 从背包中移除一定数量的指定物品
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemID"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveItemFromBag(ItemType itemType, string itemID, int amount)
    {
        if (GetItemAmount(itemType, itemID) < amount) return false;
        else
        {
            bagDataDic[itemType][itemID] -= amount;
            if(bagDataDic[itemType].Count == 0) bagDataDic.Remove(itemType);
            SaveBagDataDic();
            return true;
        }
    }

    /// <summary>
    /// 获取背包中某一物品的剩余量
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public int GetItemAmount(ItemType itemType, string itemID)
    {
        if (bagDataDic.ContainsKey(itemType) && bagDataDic[itemType].ContainsKey(itemID))
        {
            return bagDataDic[itemType][itemID];
        }
        else
        {
            return 0;
        }
    }

    public void LoadBagDataDic()
    {
        bagDataDic = BinaryDataManager.Instance.LoadDataFromFile<SerializableDictionary<ItemType, SerializableDictionary<string, int>>>("Data/Bag/", "BagDataDic");
        if(bagDataDic == null) bagDataDic = new SerializableDictionary<ItemType, SerializableDictionary<string, int>>();
    }

    public void SaveBagDataDic()
    {
        BinaryDataManager.Instance.SaveDataToFile("Data/Bag/", "BagDataDic", bagDataDic);
    }

    public IEnumerator<KeyValuePair<ItemType, SerializableDictionary<string, int>>> GetEnumerator()
    {
        foreach (KeyValuePair<ItemType, SerializableDictionary<string, int>> kvp in bagDataDic)
        {
            yield return kvp;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
