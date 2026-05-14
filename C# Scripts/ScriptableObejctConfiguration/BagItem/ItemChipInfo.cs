using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo/New ChipType", fileName = "Assets/Configurations/ItemInfos/ItemChip/New ChipType")]
public class ItemChipInfo : ItemInfoBase
{
    public Sprite chipIcon;
    public ChipType chipType;
    public float chipNumericalValue;
    
    public override void Use()
    {
        base.Use();
    }

    public override bool Buy(int amount)
    {
        return false;
    }

    public float GetChipNumericalValue(ChipType chipType)
    {
        if (this.chipType == chipType)
        {
            return this.chipNumericalValue;
        }
        else
        {
            return 0;
        }
    }
}
