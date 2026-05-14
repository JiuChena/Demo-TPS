using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public abstract class InteractionBasicModule : MonoBehaviour
{
    public bool intercatived = true;
    
    [Header("触发器组件")] public BoxCollider triggerBox;
    [Header("交互范围")] public Vector3 size = Vector3.one;
    [Header("触发器位置偏移")] public Vector3 triggerOffset = Vector3.zero;
    
    #region 周期函数

    private void Start()
    {
        if(triggerBox == null) (triggerBox = this.AddComponent<BoxCollider>()).isTrigger = true;
        
        Init();
    }

    private void Update()
    {
        SetTriggerSizeAndOffset();
        
        OnUpdateFunction();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (Application.isPlaying)
        {
            if (FilterDetection(other.gameObject))
            {
                TriggerEnterPerformance(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Application.isPlaying)
        {
            if (FilterDetection(other.gameObject))
            {
                TriggerStayPerformance(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Application.isPlaying)
        {
            if (FilterDetection(other.gameObject))
            {
                TriggerExitPerformance(other.gameObject);
            }
        }
    }

    #endregion

    #region 私有方法

    //设置触发器尺寸，位置
    private void SetTriggerSizeAndOffset()
    {
        Vector3 localScale = transform.localScale;
        triggerBox.size = new Vector3(size.x / localScale.x, size.y / localScale.y, size.z / localScale.z);
        triggerBox.center = triggerOffset;
    }

    #endregion

    #region 可继承多态方法

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected virtual void Init()
    {
        
    }

    protected virtual void OnUpdateFunction()
    {
        if(!Application.isPlaying) return;
    }

    /// <summary>
    /// 物体进入时的过滤函数
    /// </summary>
    /// <param name="target"></param>
    protected virtual bool FilterDetection(GameObject target)
    {
        return target.layer == LayerMask.NameToLayer("Player");
    }
    
    /// <summary>
    /// 物体进入触发域的执行函数
    /// </summary>
    /// <param name="target"></param>
    protected virtual void TriggerEnterPerformance(GameObject target)
    {
        
    }
    
    /// <summary>
    /// 物体持续处于触发域的执行函数
    /// </summary>
    /// <param name="target"></param>
    protected virtual void TriggerStayPerformance(GameObject target)
    {
        
    }
    
    /// <summary>
    /// 物体离开触发域的执行函数
    /// </summary>
    /// <param name="target"></param>
    protected virtual void TriggerExitPerformance(GameObject target)
    {
        
    }

    #endregion
    
}
