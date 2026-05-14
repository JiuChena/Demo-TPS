using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class OptionInteractionBase : InteractionBasicModule
{
    #region 参数

    [Header("选项内容")] public string optionContent;
    [Header("是否可重复按下")] public bool repeat = true;
    
    //是否已经触发过
    private bool haveTriggered = false;

    //事件唯一标识ID
    private string ID;

    #endregion

    #region 多态继承方法

    protected override void Init()
    {
        ID = this.gameObject.name + IDDistributor.Instance.GetID().ToString();
    }

    protected override void TriggerEnterPerformance(GameObject target)
    {
        base.TriggerEnterPerformance(target);

        if (repeat || !haveTriggered)
        {
            if (PanelManager.Instance.GetPanel<InteractionPanel>("Interaction Panel") != null)
            {
                PanelManager.Instance.GetPanel<InteractionPanel>("Interaction Panel").AddEvent(ID, optionContent, InteractionAction);
            }
        }
    }

    protected override void TriggerExitPerformance(GameObject target)
    {
        base.TriggerExitPerformance(target);
        
        PanelManager.Instance.GetPanel<InteractionPanel>("Interaction Panel").RemoveEvent(ID);
    }

    protected abstract void ActionTrigger();

    #endregion

    #region 非继承私有化方法

    //交互事件
    private void InteractionAction(InputAction.CallbackContext context)
    {
        ActionTrigger();
        
        ActionEnd();
    }

    //事件触发后检测是否可重复交互
    private void ActionEnd()
    {
        if (!repeat)
        {
            haveTriggered = true;
            PanelManager.Instance.GetPanel<InteractionPanel>("Interaction Panel").RemoveEvent(ID);
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 重置触发状态，使事件可被再次触发
    /// </summary>
    public void ResetTriggerState()
    {
        haveTriggered = false;
    }

    #endregion
}
