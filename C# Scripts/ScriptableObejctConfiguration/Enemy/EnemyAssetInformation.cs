using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Assets/Configurations/Asset Information/Enemy/New Enemy Asset Information", menuName = "Create New Config/Enemy/Enemy Information")]
public class EnemyAssetInformation : UnitAssetInfoBase
{
    public EnemyType enemyType;
    public ENNumericalConfiguration dataBase;
    public ENGrowthConfiguration dataGrowth;
    public EnemySkillDataTable skillDataTable;
}
