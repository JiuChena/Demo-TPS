using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using XLua;

public class CharacterAnimatorDriver : AnimatorDriverBase
{
    public CharacterAssetInforamtion CHAssetInfo;
    [HideInInspector] public CharacterController cc;

    [Header("特效生成位置配置")] 
    public List<ParticleSystem> fireEffects = new List<ParticleSystem>();

    public BehaviorContext behaviorContext;

    private UnityAction<BehaviorContext> CHAttack;
    private UnityAction<BehaviorContext> CHTalent;
    private UnityAction<BehaviorContext> CHBurst;

    private void Awake()
    {
        hsm = new HSM(this);
        inputData.ResetInput();
        cc = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        
        hsm.AddState<PlayerIdleState>(new PlayerIdleState(hsm));
        hsm.AddState<PlayerAttackState>(new PlayerAttackState(hsm));
        hsm.AddState<PlayerReloadState>(new PlayerReloadState(hsm));
        hsm.AddState<PlayerSkillState>(new PlayerSkillState(hsm));
        hsm.AddState<PlayerMoveState>(new PlayerMoveState(hsm));
        hsm.AddState<PlayerDeathState>(new PlayerDeathState(hsm));

        //加载角色技能Lua文件
    }

    private void Start()
    {
        Debug.Log($"{CHAssetInfo.assetID}".Substring(0,6) + "Behavior");
        LuaEnvManager.Instance.DoLua($"{CHAssetInfo.assetID}".Substring(0,6) + "Behavior");
        CHAttack = LuaEnvManager.Instance.Global.Get<UnityAction<BehaviorContext>>($"{CHAssetInfo.assetID}".Substring(0, 6) + "_Attack");
        CHTalent = LuaEnvManager.Instance.Global.Get<UnityAction<BehaviorContext>>($"{CHAssetInfo.assetID}".Substring(0,6) + "_Talent");
        CHBurst = LuaEnvManager.Instance.Global.Get<UnityAction<BehaviorContext>>($"{CHAssetInfo.assetID}".Substring(0, 6) + "_Burst");

        behaviorContext.actualData = PlayerControlModule.Instance.GetCHActualDataPanel;
    }

    private void OnEnable()
    {
        hsm.SwitchState<PlayerIdleState>();
    }

    private void Update()
    {
        hsm.StateOnUpdate();
        
        death = inputData.death;
    }

    public void Attack_Fire()
    {
        CHAttack(behaviorContext);
    }

    public void TalentSkill()
    {
        CHTalent(behaviorContext);
    }

    public void BurstSkill()
    {
        CHBurst(behaviorContext);
        Debug.Log("调用一次爆发技能效果");
    }
}
