using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CESODataGrowth : ScriptableObject
{
    [Header("基础数值区")]
    [Tooltip("生命值成长度")] public float healthGrowth = 0.2f;
    [Tooltip("攻击力成长度")] public float attackGrowth = 0.2f;
    [Tooltip("防御力成长度")] public float defenceGrowth = 0.2f;
}
