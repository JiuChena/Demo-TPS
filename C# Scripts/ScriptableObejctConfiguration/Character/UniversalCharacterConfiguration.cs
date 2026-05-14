using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/Asset Information/Character/Universal/New UCC", menuName = "Create New Config/Character/Universal Character Configuration")]
public class UniversalCharacterConfiguration : ScriptableObject
{
    [Header("特效配置")] 
    public GameObject buff;
    public GameObject debuff;
    public GameObject healCast;
    public GameObject healLoop;
    
    [Header("音效配置")]
    public AudioClip footstepSound;
}
