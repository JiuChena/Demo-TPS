using System.Collections.Generic;
using UnityEngine;

public class LocalTaskData
{
    private List<string> taskIDs = new List<string>();
    
    public List<string> Data { get { return taskIDs; } }

    private static string LOCAL_TASKS_DATA_PATH = "/Data/Tasks/";
    private static string LOCAL_TASKS_DATA_FILENAME = "LocalTaskData";

    public void LoadData()
    {
        taskIDs = BinaryDataManager.Instance.LoadDataFromFile<List<string>>(LOCAL_TASKS_DATA_PATH, LOCAL_TASKS_DATA_FILENAME) ?? new List<string>();
    }

    public void SaveData()
    {
        BinaryDataManager.Instance.SaveDataToFile(LOCAL_TASKS_DATA_PATH, LOCAL_TASKS_DATA_FILENAME, taskIDs);
    }

    public void AddTask(GameTaskConfig taskConfig)
    {
        if (!taskIDs.Contains(taskConfig.name))
        {
            taskIDs.Add(taskConfig.name);
            SaveData();
        }
    }

    public void RemoveTask(string taskID)
    {
        if (taskIDs.Contains(taskID))
        {
            taskIDs.Remove(taskID);
            SaveData();
        }
    }
}
