using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InteractionPanel : PanelBase
{
    public InputActionReference scrollWheelAction;
    public InputActionReference interactionPress;
    public Transform content;
    public Transform arrow;
    private Vector3 arrowPos;
    
    private bool firstDisplay = true;
    private int previousOptionIndex = 0;
    private int currentOptionIndex = 0;
    private int index = 0;
    
    private List<string> keys = new List<string>();
    private List<InteractionOption> options = new List<InteractionOption>();
    private List<Action<InputAction.CallbackContext>> actions = new List<Action<InputAction.CallbackContext>>();
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        if(scrollWheelAction == null) Debug.LogError("鼠标滚轮监听为空");
        if(interactionPress == null) Debug.LogError("交互按键监听为空");
    }

    protected override void OnUpdate()
    {
        if (actions.Count > 0)
        {
            int scr = (int)scrollWheelAction.action.ReadValue<float>();
        
            index = Mathf.Clamp(index + scr, 0, actions.Count - 1);

            if (currentOptionIndex != index || firstDisplay)
            {
                try
                {
                    previousOptionIndex = currentOptionIndex;

                    currentOptionIndex = index;

                    //使上一个选项设为未被选中状态，当前选项为选中状态
                    options[previousOptionIndex].OptionUnselect();
                    interactionPress.action.performed -= actions[previousOptionIndex];

                    options[currentOptionIndex].OptionSelect();
                    interactionPress.action.performed += actions[currentOptionIndex];

                    arrowPos = new Vector3(0, options[currentOptionIndex].transform.localPosition.y, 0);
                    arrow.gameObject.SetActive(true);
                }
                catch
                {
                    return;
                }
                
                firstDisplay = false;
            }
        }
        else
        {
            IndexInit();
        }
        
        arrow.localPosition = Vector3.Lerp(arrow.localPosition, arrowPos, 0.5f);
        
        AddEventHandler();

        RemoveEventHandler();
    }

    private void IndexInit()
    {
        index = 0;
        currentOptionIndex = 0;
        previousOptionIndex = 0;
        firstDisplay = true;
        arrow.gameObject.SetActive(false);
    }
    
    private Queue<ReadyAddEvent> readyAddQueue = new Queue<ReadyAddEvent>();

    /// <summary>
    /// 添加选项事件
    /// </summary>
    public void AddEvent(string eventID, string interactionText, Action<InputAction.CallbackContext> action)
    {
        if (!keys.Contains(eventID))
        {
            readyAddQueue.Enqueue(new ReadyAddEvent(eventID, interactionText, action));
        }
    }

    private ReadyAddEvent addEvent;
    
    private void AddEventHandler()
    {
        if (readyAddQueue.Count > 0)
        {
            addEvent = readyAddQueue.Dequeue();
            
            if(keys.Contains(addEvent.eventID)) return;
            
            keys.Add(addEvent.eventID);
            actions.Add(addEvent.action);
            
            ObjectsPool.Instance.GetObjectFromPool("Interaction Option", content, (obj) =>
            {
                InteractionOption option = obj.GetComponent<InteractionOption>();
                obj.transform.localScale = Vector3.one;
                option.OptionInit(addEvent.interactionText);
                options.Add(option);

                IndexInit();
            });
        }
    }
    
    private Queue<string> readyRemoveEvent = new Queue<string>();

    /// <summary>
    /// 移除选项事件
    /// </summary>
    public void RemoveEvent(string eventID)
    {
        if (keys.Contains(eventID))
        {
            readyRemoveEvent.Enqueue(eventID);
        }
    }
    
    private async void RemoveEventHandler()
    {
        if (readyRemoveEvent.Count > 0)
        {
            string eventID = readyRemoveEvent.Dequeue();
            
            if(!keys.Contains(eventID)) return;
            
            int keyIndex = keys.IndexOf(eventID);
            
            options[keyIndex].OptionUnselect();
            ObjectsPool.Instance.ReturnObjectToPool(options[keyIndex].gameObject);

            if (keyIndex == currentOptionIndex) interactionPress.action.performed -= actions[currentOptionIndex];
            
            keys.RemoveAt(keyIndex);
            options.RemoveAt(keyIndex);
            actions.RemoveAt(keyIndex);
            
            IndexInit();
        }
    }
}

public class ReadyAddEvent
{
    public string eventID;
    public string interactionText;
    public Action<InputAction.CallbackContext> action;

    public ReadyAddEvent(string eventID, string interactionText, Action<InputAction.CallbackContext> action)
    {
        this.eventID = eventID;
        this.interactionText = interactionText;
        this.action = action;
    }
}