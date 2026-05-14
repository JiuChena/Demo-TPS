using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TeamConfiguration
{
    public List<CharacterAssetInforamtion> CHAssetInfos = new List<CharacterAssetInforamtion>();
    
    public TeamConfiguration() {}

    public TeamConfiguration(List<CharacterAssetInforamtion> chAssetInfos)
    {
        this.CHAssetInfos = chAssetInfos;
    }
    
    public TeamCharacterIDs TeamCHIDs = new TeamCharacterIDs();
    
    public IEnumerator LoadTeamConfiguration(UnityAction<List<CharacterAssetInforamtion>> callback = null)
    {
        ReleaseCHAssetInfos();
        
        TeamCharacterIDs newTeamCHIDs = new TeamCharacterIDs();

        newTeamCHIDs.LoadCHIDsFromFile();

        for (int i = 0; i < newTeamCHIDs.CHIDs.Count; i++) CHAssetInfos.Add(null);

        for (int i = 0; i < newTeamCHIDs.CHIDs.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadAssetAsync<CharacterAssetInforamtion>(newTeamCHIDs.CHIDs[index], (result) =>
            {
                CHAssetInfos[index] = result;
            });
        }
        
        ReturnCheck:

        for (int i = 0; i < newTeamCHIDs.CHIDs.Count; i++)
        {
            AsyncOperationStatus status = AddressableManager.Instance.GetResourceStatus<CharacterAssetInforamtion>(newTeamCHIDs.CHIDs[i]);
            
            switch (status)
            {
                case AsyncOperationStatus.Succeeded:
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogWarning("加载失败！");
                    break;
                case AsyncOperationStatus.None:
                    Debug.Log("资源加载中");
                    // PanelManager.Instance.PanelDisplay<LoadingPanel>("Loading Panel", UILayer.Top);
                    yield return null;
                    goto ReturnCheck;
            }
        }
        
        callback(CHAssetInfos);
    }

    public void SaveTeamConfiguration()
    {
        TeamCHIDs.SaveCHIDsToFile(this.CHAssetInfos);
    }

    private void ReleaseCHAssetInfos()
    {
        for (int i = 0; i < this.CHAssetInfos.Count; i++)
        {
            AddressableManager.Instance.ReleaseResource<CharacterAssetInforamtion>(CHAssetInfos[i].assetID);
        }
        
        CHAssetInfos.Clear();
    }
}

[Serializable]
public class TeamCharacterIDs
{
    public List<string> CHIDs = new List<string>();

    public void SaveCHIDsToFile(List<CharacterAssetInforamtion> CHIDs)
    {
        for (int i = 0; i < CHIDs.Count; i++)
        {
            this.CHIDs.Add(CHIDs[i].assetID);
        }
        
        BinaryDataManager.Instance.SaveDataToFile("Team/", "TeamConfiguration", this.CHIDs);
    }
    
    public void SaveCHIDsToFile(List<string> CHIDs)
    {
        this.CHIDs = CHIDs;
        
        BinaryDataManager.Instance.SaveDataToFile("Team/", "TeamConfiguration", this.CHIDs);
    }

    public void LoadCHIDsFromFile()
    {
        this.CHIDs = BinaryDataManager.Instance.LoadDataFromFile<List<string>>("Team/", "TeamConfiguration");
    }
}
