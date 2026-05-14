using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHChipsDataDic : IEnumerable<KeyValuePair<string, CHChipsData>>
{
    private SerializableDictionary<string, CHChipsData> chipsDataDic = new SerializableDictionary<string, CHChipsData>();

    public void LoadData()
    {
        chipsDataDic = BinaryDataManager.Instance.LoadDataFromFile<SerializableDictionary<string, CHChipsData>>("Data/Character/", "CHChipsDataDic");
        chipsDataDic = chipsDataDic == null ? new SerializableDictionary<string, CHChipsData>() : chipsDataDic;
    }

    public void SaveData()
    {
        BinaryDataManager.Instance.SaveDataToFile("Data/Character/", "CHChipsDataDic", chipsDataDic);
    }
    
    public void UpdateCHTopChipData(string CHID, ItemChipInfo topChipInfo)
    {
        chipsDataDic[CHID].topChipID = topChipInfo.name;
        
    }

    public void UpdateCHBotChipData(string CHID, ItemChipInfo botChipInfo)
    {
        chipsDataDic[CHID].botChipID = botChipInfo.name;
    }

    public CHChipsData GetCHChipsData(string CHID)
    {
        if (chipsDataDic.ContainsKey(CHID))
        {
            return chipsDataDic[CHID];
        }
        else
        {
            // Debug.Log($"不存在ID为{CHID}的角色");
            return new CHChipsData();
        }
    }

    public IEnumerator<KeyValuePair<string, CHChipsData>> GetEnumerator()
    {
        foreach (KeyValuePair<string, CHChipsData> chip in chipsDataDic)
        {
            yield return chip;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
