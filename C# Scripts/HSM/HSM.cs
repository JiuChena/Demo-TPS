using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HSM
{
    private AnimatorDriverBase driverBase;
    public CharacterAnimatorDriver CHDriver;
    public EnemyAnimatorDriver EnemyDriver;
    
    public HSM(CharacterAnimatorDriver chDriver){ this.CHDriver = chDriver; driverBase = chDriver; }
    public HSM(EnemyAnimatorDriver enemyAnimatorChDriver) { this.EnemyDriver = enemyAnimatorChDriver; driverBase = enemyAnimatorChDriver; }
    
    private Dictionary<Type, StateBase> states = new Dictionary<Type, StateBase>();
    
    private StateBase currentState;
    private StateBase previousState;

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public StateBase GetCurrentState
    {
        get
        {
            if (currentState == null)
            {
                return null;
            }
            else
            {
                return currentState;
            }
        }
    }

    /// <summary>
    /// 添加一个状态
    /// </summary>
    /// <param name="stateBase"></param>
    /// <typeparam name="T"></typeparam>
    public void AddState<T>(StateBase stateBase) where T : StateBase
    {
        Type stateType = typeof(T);
        if (states.ContainsKey(stateType))
        {
            return;
        }
        else
        {
            states.Add(stateType, stateBase);
        }
    }

    /// <summary>
    /// 移除一个状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemoveState<T>() where T : StateBase
    {
        Type stateType = typeof(T);
        if (states.ContainsKey(stateType))
        {
            states.Remove(stateType);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void SwitchState<T>() where T : StateBase
    {
        Type stateType = typeof(T);

        if (states.ContainsKey(stateType))
        {
            if (currentState != states[stateType])
            {
                previousState = currentState;
                currentState = states[stateType];
                
                previousState?.OnExit();
                currentState?.OnEnter();
            }
        }
    }

    /// <summary>
    /// 当前状态帧更新
    /// </summary>
    public void StateOnUpdate()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
            
            if(driverBase.stateDebug) Debug.Log(currentState.GetType());
        }
    }
}
