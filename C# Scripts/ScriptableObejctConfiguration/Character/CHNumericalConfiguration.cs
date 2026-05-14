using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/CEData Base/Character/New CHNC", menuName = "Create New Config/Character/CHNumericalConfiguration")]
public class CHNumericalConfiguration : CESODataBase
{
    [Tooltip("天赋技冷却时间")] public float talentCooltime = 10f;
}
