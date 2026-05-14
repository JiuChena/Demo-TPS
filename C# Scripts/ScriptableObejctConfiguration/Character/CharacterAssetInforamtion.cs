using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Assets/Configurations/Asset Information/Character/New Character Asset Information", menuName = "Create New Config/Character/Character Information")]
public class CharacterAssetInforamtion : UnitAssetInfoBase
{
    [Space(10)] [Header("通用配置")]
    public UniversalCharacterConfiguration universalConfiguration;

    [Space(10)] [Header("音效配置")] 
    public AudioClip attackDely;
    public AudioClip reload;
    
    [Space(10)]
    [Header("数值配置")] 
    [Tooltip("基础数值配置文件")] public CHNumericalConfiguration dataBase;
    [Tooltip("成长度数值配置文件")] public CESODataGrowth dataGrowth;

    [Space(10)] [Header("图标配置")] 
    public Sprite CHProfilePictureSprite;
    public Sprite CHBurstSprite;
    public Sprite CHTalentSprite;
    public Texture2D mouseTexture;
    
    public CHSkillDataTable chSkillDataTable;
    
    public void DataInitialize()
    {
        if (modle == null)
        {
            Debug.LogError("请确保对应预制体模型不为空再尝试初始化!");
            return;
        }
        
        assetID = modle.name;
        name = modle.name.Substring(6, modle.name.Length - 6);
    }
}