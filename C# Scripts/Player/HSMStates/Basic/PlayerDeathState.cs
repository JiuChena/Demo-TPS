using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : StateBase
{
    public PlayerDeathState(HSM hsm) : base(hsm)
    {
        
    }

    public override void OnEnter()
    {
        hsm.CHDriver.animator.SetTrigger("Death");
        //判断应该是哪一种死亡状态,之后判断当动画播放完毕时对材质球属性进行修改使得材质溶解,溶解完成之后隐藏该角色模型并且把溶解度重新调回1
        //如果需要复活阵亡角色,那么需要重置animationdriver中的death状态,然后重新给对应的curHealth赋值即可
    }

    public override void OnUpdate()
    {
        if(!hsm.CHDriver.animator.GetCurrentAnimatorStateInfo(0).IsName("Vital_Death") || hsm.CHDriver.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) return;
        
        //材质球调参
        
        //判断是否调参完毕
        
        PlayerControlModule.Instance.CurCHDeath();
    }

    public override void OnExit()
    {
        
    }
}
