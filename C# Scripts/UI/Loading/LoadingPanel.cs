using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadingPanel : PanelBase
{
    public LoadingPanelType loadType;
    
    public Animator circleLoadAnimator;
    [FormerlySerializedAs("fadeLoadAnimator")] public Animator colorLoadAnimator;

    private Animator animator;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        
    }

    protected override void OnUpdate()
    {
        
    }

    public override void DisplayPanel()
    {
        switch (loadType)
        {
            case LoadingPanelType.Circle:
                animator = circleLoadAnimator;
                break;
            case LoadingPanelType.Color:
                animator = colorLoadAnimator;
                break;
        }
        
        animator.gameObject.SetActive(true);
        animator?.SetBool("Display", true);
    }

    public override void HidePanel()
    {
        animator?.SetBool("Display", false);
        
        DestroyPanel(0.5f);
    }
}
