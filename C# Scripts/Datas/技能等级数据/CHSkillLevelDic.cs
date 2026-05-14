using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHSkillLevelDic
{
    private SerializableDictionary<string, CHSkillLevelData> CHSkillLevelDicData = new SerializableDictionary<string, CHSkillLevelData>();

    public void LoadData()
    {
        CHSkillLevelDicData = BinaryDataManager.Instance.LoadDataFromFile<SerializableDictionary<string, CHSkillLevelData>>("Data/LevelDic/", "CHSkillLevelDicData");
        CHSkillLevelDicData = CHSkillLevelDicData == null ? new SerializableDictionary<string, CHSkillLevelData>() : CHSkillLevelDicData;
    }

    public void SaveData()
    {
        BinaryDataManager.Instance.SaveDataToFile("Data/LevelDic/", "CHSkillLevelDicData", CHSkillLevelDicData);
    }

    public CHSkillLevelData GetCHSkillLevelData(string CHName)
    {
        if (CHSkillLevelDicData.ContainsKey(CHName))
        {
            return CHSkillLevelDicData[CHName];
        }
        else
        {
            return null;
        }
    }

    public bool AddCHSkillLevelData(string CHName)
    {
        if (!CHSkillLevelDicData.ContainsKey(CHName))
        {
            CHSkillLevelDicData.Add(CHName, new CHSkillLevelData());
            SaveData();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveCHSkillLevelData(string CHName)
    {
        if (CHSkillLevelDicData.ContainsKey(CHName))
        {
            CHSkillLevelDicData.Remove(CHName);
            SaveData();
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class CHSkillLevelData
{
    public int attackLevel = 1;
    public int talentLevel = 1;
    public int burstLevel = 1;
}
