using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDicData : IEnumerable<KeyValuePair<string, int>>
{
    public static string LEVEL_DIC_PATH = "Data/LevelDic/";
    public static string LEVEL_DIC_FILENAME = "CHLevelDic";
    
    private SerializableDictionary<string, int> levelDic = new SerializableDictionary<string, int>();

    public static int LevelTop = 80;
    public int GetLevel(string CHName)
    {
        if (levelDic.ContainsKey(CHName))
        {
            return levelDic[CHName];
        }
        else
        {
            Debug.LogWarning($"当前所记录等级字典中不包含名为{CHName}的角色，无法获取！");
            return 0;
        }
    }

    public bool AddCH(string CHName)
    {
        if (!levelDic.ContainsKey(CHName))
        {
            levelDic.Add(CHName, 1);
            SaveLevelDic();
            return true;
        }
        else
        {
            Debug.LogWarning($"名为{CHName}的角色已存在于等级字典，请不要重复添加！");
            return false;
        }
    }

    public void RemoveCH(string CHName)
    {
        if (levelDic.ContainsKey(CHName))
        {
            levelDic.Remove(CHName);
            SaveLevelDic();
        }
        else
        {
            Debug.LogWarning($"当前所记录等级字典中不包含名为{CHName}的角色，无法移除！");
        }
    }

    public bool ModifyCHLevel(string CHName, int level)
    {
        if (levelDic.ContainsKey(CHName))
        {
            if (levelDic[CHName] + level < LevelTop)
            {
                levelDic[CHName] += level;
                SaveLevelDic();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"当前所记录等级字典中不包含名为{CHName}的角色，无法修改角色等级！");
            return false;
        }
    }

    public void SaveLevelDic()
    {
        BinaryDataManager.Instance.SaveDataToFile(LEVEL_DIC_PATH, LEVEL_DIC_FILENAME, levelDic);
    }

    public void LoadLevelDic()
    {
        this.levelDic = BinaryDataManager.Instance.LoadDataFromFile<SerializableDictionary<string, int>>(LEVEL_DIC_PATH, LEVEL_DIC_FILENAME);
    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
        foreach (KeyValuePair<string, int> info in levelDic)
        {
            yield return info;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
