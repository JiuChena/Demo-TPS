using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class TaskPanel : PanelBase
{
    public RectTransform content;
    public Button closeButton;
    
    //加载所有的未完成任务列表，获取对应的任务列表显示函数
    protected override void LoadInit()
    {
        
    }
    
    [CSharpCallLua] public delegate void TaskDisplayAction(TMP_Text textComponent);

    private int i = 0;
    private GameTaskConfig tempConfig;
    protected override void CompomentInit()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.PanelHide("Task Panel");
        });
        
        //拿到本地任务列表数据
        for ( ; i < TaskSystem.Instance.taskData.Data.Count; i++)
        {
            tempConfig = AddressableManager.Instance.GetResource<GameTaskConfig>(TaskSystem.Instance.taskData.Data[i]);
            
            //获取名字，描述，奖励描述组件，把对应东西填充进去，获取对应任务显示函数
            string taskName = tempConfig.taskName;
            string taskDescription = tempConfig.taskDescription;
            string taskRewardDescription = tempConfig.taskRewardDescription;

            TaskDisplayAction action = LuaEnvManager.Instance.Global.Get<TaskDisplayAction>(tempConfig.taskListDisplayFuncName);
            
            //创建任务UI单元
            ObjectsPool.Instance.GetObjectFromPool("Task Item", content, (obj) =>
            {
                obj.transform.localScale = Vector3.one;
                obj.transform.Find("TaskName").GetComponent<TMP_Text>().text = taskName;
                obj.transform.Find("TaskDescription").GetComponent<TMP_Text>().text = taskDescription;
                obj.transform.Find("TaskRewardDescription").GetComponent<TMP_Text>().text = taskRewardDescription;
                
                action?.Invoke(obj.transform.Find("TaskProcess").GetComponent<TMP_Text>());
            });
            
            AddressableManager.Instance.ReleaseResource<GameTaskConfig>(name);
        }
        
        content.sizeDelta = new Vector2(0, 50 + 250 * i);
    }

    protected override void OnUpdate()
    {
        
    }

    public override void DisplayPanel()
    {
        PlayerControlModule.Instance.PlayerControlDisable();
        Camera.main.GetComponent<GaussianBlur>().enabled = true;
    }

    public override void HidePanel()
    {
        PlayerControlModule.Instance.PlayerControlEnable();
        Camera.main.GetComponent<GaussianBlur>().enabled = false;
        
        DestroyPanel();
    }
}
