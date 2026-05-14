using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAssetInfoBase : ScriptableObject
{
    [Header("基础信息配置")]
    public string assetID;
    public string name;
    public GameObject modle;
    public SpecialActionAcpability specialAction;
    public WeaponType WeaponType;
    public int ammunitionCapacity;
    public List<Material> materials;
}