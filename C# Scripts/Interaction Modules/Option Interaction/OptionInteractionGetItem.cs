using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionInteractionGetItem : OptionInteractionBase
{
    public SerializableDictionary<ItemType, SerializableDictionary<string, int>> itemsGot = new SerializableDictionary<ItemType, SerializableDictionary<string, int>>();

    protected override bool FilterDetection(GameObject target)
    {
        return target.layer == LayerMask.NameToLayer("Player");
    }

    protected override void ActionTrigger()
    {
        foreach (KeyValuePair<ItemType, SerializableDictionary<string, int>> kvp1 in itemsGot)
        {
            foreach (KeyValuePair<string, int> kvp2 in kvp1.Value)
            {
                DataCenter.Instance.bagDataDic.AddItemToBag(kvp1.Key, kvp2.Key, kvp2.Value);
            }
        }
    }
}
