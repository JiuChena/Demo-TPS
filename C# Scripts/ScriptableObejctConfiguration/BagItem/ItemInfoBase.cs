using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoBase : ScriptableObject
{
    [Header("物品设置")]
    public bool canBeUsed = false;
    public bool canBeBought = true;
    [Header("物品信息")]
    public string itemName;
    public int value;
    public string itemDescription;
    public List<string> sources;

    public virtual bool Buy(int amount)
    {
        return false;
    }

    public virtual void Use()
    {
        
    }
}
