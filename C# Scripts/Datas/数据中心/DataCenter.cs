using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    private static DataCenter instance;
    public static DataCenter Instance => instance;

    private void Awake()
    {
        instance = this;
    }
    
    public BagDataDic bagDataDic = new BagDataDic();
    
    public StoreData storeData = new StoreData();
    
    public CHChipsDataDic chipsDataDic = new CHChipsDataDic();
    
    public CHSkillLevelDic chSkillLevelDic = new CHSkillLevelDic();

    private void Start()
    {
        BagDataTest();

        StoreDataTest();
        
        CHChipsDataTest();

        CHSkillLevelTest();
    }

    private void BagDataTest()
    {
        bagDataDic.LoadBagDataDic();
        // bagDataDic.AddItemToBag(ItemType.Money, "Money", 10000);
        
        // bagDataDic.AddItemToBag(ItemType.Ammunition, "Ammo-7.62", 100);
        // bagDataDic.AddItemToBag(ItemType.Potion, "Inferior Heal Potion", 10);
        // bagDataDic.AddItemToBag(ItemType.Prop, "Experience", 100);
        // bagDataDic.AddItemToBag(ItemType.Chip, "Inferior Attack Chip", 1);
        // bagDataDic.AddItemToBag(ItemType.Potion, "Inferior Attack Potion", 10);
        
        bagDataDic.SaveBagDataDic();
    }

    private void StoreDataTest()
    {
        storeData.LoadData();
    }

    private void CHChipsDataTest()
    {
        chipsDataDic.LoadData();
    }

    private void CHSkillLevelTest()
    {
        chSkillLevelDic.LoadData();
    }
}
