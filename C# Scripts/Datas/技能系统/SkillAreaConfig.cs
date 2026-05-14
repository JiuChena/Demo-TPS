using System;
using UnityEngine;

[Serializable]
public class SkillAreaConfig
{
    [Tooltip("技能范围")] public SkillArea burstSkillArea;
    
    [ShowIf("burstSkillArea", SkillArea.Sector)] public float radius;
    [ShowIf("burstSkillArea", SkillArea.Sector)] public float angle;
    
    [ShowIf("burstSkillArea", SkillArea.Rectangle)] public float length;
    [ShowIf("burstSkillArea", SkillArea.Rectangle)] public float width;
}