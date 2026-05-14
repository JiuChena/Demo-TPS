using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public class TaskSystem : MonoBehaviour
{
    private static TaskSystem instance;

    public static TaskSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                go.name = "TaskSystem";
                instance = go.AddComponent<TaskSystem>();
            }
            
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        StartCoroutine(Init());
    }

    public PreLoadTaskLuaScriptsTable table;
    
    [CSharpCallLua] public delegate void TaskAction(params object[] args);

    //任务列表，任务函数(根据配置表读取Lua脚本中的对应函数)，任务状态，任务名称
    public LocalTaskData taskData;
    private Dictionary<string, TaskAction> events = new Dictionary<string, TaskAction>();

    public IEnumerator Init()
    {
        taskData = new LocalTaskData();
        taskData.LoadData();

        for (int i = 0; i < table.luaScriptNames.Count; i++)
        {
            LuaEnvManager.Instance.DoLua(table.luaScriptNames[i]);
        }
        
        //根据本地数据为
        for (int i = 0; i < taskData.Data.Count; i++)
        {
            //本地存储了任务ID
            //需要加载出任务配置表，然后根据配置表执行Lua脚本，根据配置表获取对应函数
            int index = i;
            AddressableManager.Instance.LoadAssetAsync<GameTaskConfig>(taskData.Data[index], (config) =>
            {
                events.Add(config.name, LuaEnvManager.Instance.Global.Get<TaskAction>(config.taskListenerFuncName));
            });

            if (i % 10 == 0) yield return null;
        }
    }

    public void InsertTask(string taskID)
    {
        if (!events.ContainsKey(taskID))
        {
            AddressableManager.Instance.LoadAssetAsync<GameTaskConfig>(taskID, (config) =>
            {
                TaskAction action = LuaEnvManager.Instance.Global.Get<TaskAction>(config.taskListenerFuncName);
                events.Add(config.name, action);
                
                taskData.AddTask(config);
            });
        }
    }

    public void RemoveTask(string taskID)
    {
        if (events.ContainsKey(taskID))
        {
            events.Remove(taskID);
            
            //释放资源
            AddressableManager.Instance.ReleaseResource<GameTaskConfig>(taskID);
            
            taskData.RemoveTask(taskID);
        }
    }

    public void SetTaskTrigger(string taskID, params object[] args)
    {
        if (events.ContainsKey(taskID))
        {
            events[taskID].Invoke(args);
        }
    }
}
