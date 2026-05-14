using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

[LuaCallCSharp] [ReflectionUse]
public class TimerEventManager : MonoBehaviour
{
    private static TimerEventManager instance;
    public static TimerEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                instance = obj.AddComponent<TimerEventManager>();
                obj.name = "TimerEventManager";
                DontDestroyOnLoad(obj);
            }
            
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public Queue<TimerEvent> actions = new Queue<TimerEvent>();

    public void AddTimerEvent(float timer, UnityAction action)
    {
        actions.Enqueue(new TimerEvent(timer, action));
    }

    private void Update()
    {
        int count = actions.Count;
        for (int i = 0; i < count; i++)
        {
            TimerEvent timerEvent = actions.Dequeue();
            timerEvent.timer -= Time.deltaTime;

            if (timerEvent.timer <= 0)
                timerEvent.action();
            else
                actions.Enqueue(timerEvent);
        }
    }
}

public class TimerEvent
{
    public float timer;
    public UnityAction action;

    public TimerEvent(float timer, UnityAction action)
    {
        this.timer = timer;
        this.action = action;
    }
}