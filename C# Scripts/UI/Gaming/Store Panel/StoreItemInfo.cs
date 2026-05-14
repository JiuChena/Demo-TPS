using System;
using UnityEngine;

[Serializable]
public class StoreItemInfo<T> where T : ScriptableObject
{
    public T itemInfo;
    public int itemPurchaseLimit = 10;
}
