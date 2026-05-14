using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public abstract class PanelBase : MonoBehaviour
{
    protected Animator animator
    {
        get
        {
            if(m_Animator == null) m_Animator = GetComponent<Animator>();
            
            return m_Animator;
        }
    }
    
    private Animator m_Animator;

    private void Awake()
    {
        LoadInit();
    }

    private void Start()
    {
        CompomentInit();
    }

    private void Update()
    {
        OnUpdate();
    }

    public virtual void DisplayPanel()
    {
        
    }

    public virtual void HidePanel()
    {
        
    }

    public void DestroyPanel(float time = 0)
    {
        GameObject.Destroy(this.gameObject, time);
    }

    /// <summary>
    /// 数据初始化
    /// </summary>
    protected abstract void LoadInit();

    /// <summary>
    /// 组件初始化
    /// </summary>
    protected abstract void CompomentInit();

    /// <summary>
    /// 面板帧更新刷新函数（按需重写，无需更新的面板不必覆写）
    /// </summary>
    protected virtual void OnUpdate() { }
}










