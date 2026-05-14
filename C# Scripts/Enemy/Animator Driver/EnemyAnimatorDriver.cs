using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using XLua;

public class EnemyAnimatorDriver : AnimatorDriverBase
{
    public HSM hsm;
    public EnemyAIData enemyAIData;
    public UnitActualDataPanel actualData; 
    public EnemyDamageBoost damageBoost;
    public EnemyAssetInformation assetInfo;
    
    public LayerMask targetLayer;
    public NavMeshAgent agent;
    [HideInInspector] public GameObject targetGo;
    [HideInInspector] public CharacterController cc;

    public float exitTime;
    private float _exitTime;

    public float trackingDistance = 8;
    public float initCooltime = 5;

    [Header("最大可接受寻找【障碍物】所需路径")] public float maxFindBunkerPathLength = 5f;

    [Range(1,90)] public int level = 1;

    public Renderer bodyRenderer;
    private Material bodyMaterial;
    
    private bool actualPanelHealhInit = false;
    private Image healthBarImage;
    private Transform healthBar;
    
    public BehaviorContext behaviorContext = new BehaviorContext();

    private void Awake()
    {
        enemyAIData = new EnemyAIData();
        enemyAIData.ResetState();
        enemyAIData.startPos = transform.position;
        actualData = new UnitActualDataPanel();
        actualData.talentCooltimer = initCooltime;
        actualData.bulletCount = assetInfo.ammunitionCapacity;
        damageBoost = new EnemyDamageBoost();
        damageBoost.BoostReset();
        hsm = new HSM(this);
        animator = this.GetComponent<Animator>();
        cc = this.GetComponent<CharacterController>();
        bodyMaterial = bodyRenderer.material;
        bodyMaterial.DisableKeyword("_ENABLE_XRAY");
        
        healthBar = this.transform.Find("HealthBar/Canvas/Bar").transform;
        healthBarImage = this.transform.Find("HealthBar/Canvas/Bar/Health").GetComponent<Image>();
        maxHealthImageLength = healthBarImage.rectTransform.rect.width;
        
        hsm.AddState<EnemyIdleState>(new EnemyIdleState(this.hsm));
        hsm.AddState<EnemyAttackState>(new EnemyAttackState(this.hsm));
        hsm.AddState<EnemyReloadState>(new EnemyReloadState(this.hsm));
        hsm.AddState<EnemySkillState>(new EnemySkillState(this.hsm));
        hsm.AddState<EnemyMoveState>(new EnemyMoveState(this.hsm));
        hsm.AddState<EnemyDeathState>(new EnemyDeathState(this.hsm));
        
        LuaEnvManager.Instance.DoLua($"{assetInfo.assetID}Behavior");
        attackBehavior = LuaEnvManager.Instance.Global.Get<BehaviorFunc>($"{assetInfo.assetID}_Attack");
        burstBehavior = LuaEnvManager.Instance.Global.Get<BehaviorFunc>($"{assetInfo.assetID}_Burst");
        
        behaviorContext.actualData = actualData;
    }

    private void OnEnable()
    {
        hsm.SwitchState<EnemyIdleState>();
    }

    private void Update()
    {
        NavgationUpdate();

        ActualDataUpdate();

        HealthDisplay();
        
        if (hsm.GetCurrentState != null)
        {
            hsm.StateOnUpdate();
        }
    }

    private void NavgationUpdate()
    {
        Collider[] targets = Physics.OverlapSphere(this.transform.position, trackingDistance, targetLayer);

        if (targets.Length > 0)
        {
            targetGo = targets[0].gameObject;
            bodyMaterial.EnableKeyword("_ENABLE_XRAY");
            enemyAIData.targetExist = true;
        }
        else if(enemyAIData.targetExist)
        {
            _exitTime = exitTime;
            enemyAIData.targetExist = false;
        }
        
        if (targetGo != null)
        {
            enemyAIData.targetPos = targetGo.transform.position;
            
            if (!enemyAIData.targetExist)
            {
                _exitTime -= Time.deltaTime;
                if (_exitTime <= 0)
                {
                    targetGo = null;
                    bodyMaterial.DisableKeyword("_ENABLE_XRAY");
                }
            }
        } 
    }

    private void ActualDataUpdate()
    {
        //冷却处理
        actualData.talentCooltimer = Mathf.Max(0, actualData.talentCooltimer - Time.deltaTime);
        
        //基础数值处理                        基础生命
        actualData.maxHealth = (assetInfo.dataBase.health + assetInfo.dataGrowth.healthGrowth * level) * (1 + damageBoost.healthEnhance);

        if (!actualPanelHealhInit)
        {
            actualData.curHealth = actualData.maxHealth;
            actualPanelHealhInit = true;
        }
        
        actualData.defence = (assetInfo.dataBase.defence + assetInfo.dataGrowth.defenceGrowth * level) * (1 + damageBoost.defenseEnhance);
        
        actualData.speed = assetInfo.dataBase.moveSpeed * (1 + damageBoost.speedEnhance);
        
        actualData.criticalHitRate = assetInfo.dataBase.criticalHitRate + damageBoost.criticalRateEnhance;
        
        actualData.criticalHitDamage = assetInfo.dataBase.criticalHitDamage + damageBoost.criticalDamageEnhance;
        
        actualData.damageBonus = assetInfo.dataBase.damageBonus + damageBoost.damageEnhance;

        if (actualData.curHealth == 0)
        {
            death = true;
            agent.isStopped = true;
        }
    }

    private float maxHealthImageLength;
    private void HealthDisplay()
    {
        healthBarImage.rectTransform.sizeDelta = new Vector2(maxHealthImageLength * (actualData.curHealth / actualData.maxHealth), healthBarImage.rectTransform.rect.height);
        healthBar.transform.rotation = Camera.main.transform.rotation;
    }
    
    [CSharpCallLua]
    public delegate void BehaviorFunc(BehaviorContext context);

    public BehaviorFunc attackBehavior;
    public BehaviorFunc burstBehavior;

    public void Attack()
    {
        //特效生成 + 技能效果处理（Lua）
        
        //提供SkillStartEvent，SkillDelayEvent，SkillEndEvent，SkillContinuousEvent（事件，触发间隔）
        
        //特效列表，当特效物体身上附加了技能效果处理模块时把当前索引的几个事件对应着运行即可，每个特效物体身上都附加了TheNextTriggerModule，利用Lua为它们附加判断：何时触发下一个特效
        
        //生成开火特效，生成子弹特效，击中时生成hit特效
        attackBehavior(behaviorContext);
    }

    public void Talent()
    {
        //生成
    }

    public void Burst()
    {
        //特效列表放在人物资源中，设置好区域检测形状，区域检测相关大小，持续次数，间隔时间，
        burstBehavior(behaviorContext);
    }
}

[CSharpCallLua]
[Serializable]
public class BehaviorContext
{
    public List<GameObject> objects;
    public List<Transform> transforms;
    public List<ParticleSystem> particles;
    public List<AudioClip> audioClips;
    public UnitActualDataPanel actualData;
    
    [HideInInspector] public List<GameObject> targets;
    [HideInInspector] public int count = 1;
}