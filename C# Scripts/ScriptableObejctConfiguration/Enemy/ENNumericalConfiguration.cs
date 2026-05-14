using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/CEData Base/Enemy/New CHNC", menuName = "Create New Config/Enemy/NumericalConfiguration")]
public class ENNumericalConfiguration : CESODataBase
{
    [Tooltip("天赋技冷却时间")] public float talentCooltime = 10f;
}
