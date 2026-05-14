using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerControlModule : MonoBehaviour
{
    #region 控制器实例

    private static PlayerControlModule instance;
    public static PlayerControlModule Instance => instance;

    #endregion
    
    #region 摄像机控制

    public Vector3 viewCenter = Vector3.zero;
    private int cameraIndex = 0;
    public List<GameObject> cameras = new List<GameObject>();

    #endregion

    #region 输入处理

    [Header("输入调试")] public bool inputDebuger = false;

    public InputActionAsset inputAsset;
    [HideInInspector] public PlayerInput playerInput;

    public PlayerInputData inputData = new PlayerInputData();
    
    public LayerMask mouseAttackRotateLayer;

    public float updateCHControlCooltime = 1f;
    private float updateCHControlTimer;

    #endregion

    #region 编队加载

    [Header("编队调试")] public bool TeamLoadDebuger = false;
    
    [HideInInspector] public bool loadCompleted = false;

    private int previousCHIndex = 0;
    private int currentCHIndex = 0;
    public List<CharacterAssetInforamtion> CHAssetInfos = new List<CharacterAssetInforamtion>();
    private List<CharacterAnimatorDriver> CHAnimatorDrivers = new List<CharacterAnimatorDriver>();
    private Dictionary<string, List<ItemChipInfo>> CHChipsDic = new Dictionary<string, List<ItemChipInfo>>();
    
    public CharacterAssetInforamtion defaultCHInformation;
    
    #endregion

    #region 面板数值处理

    [Header("实际面板Debug")] public bool CHActualDataDebug = false;
    //等级字典
    public LevelDicData levelDic = new LevelDicData();
    //玩家数据加成
    public BonusPanel bonusPanel = new BonusPanel();
    //插件系统加载
    private List<CHChipInfoData> CHChipInfoDatas = new List<CHChipInfoData>();
    //角色实际数据
    [HideInInspector] public List<UnitActualDataPanel> CHActualDataPanels = new List<UnitActualDataPanel>();

    [Header("能量回复效率")] public float energyEfficiency = 1;

    [Header("瞬移技能CD(S)")] public float teleportationCooltime = 15f;

    private float teleportationCooltimer;
    
    private bool initHealth = false;
    
    private bool bulletCapInit = false;

    #endregion

    #region 生命周期函数
    
    private void Awake()
    {
        instance = this;
        
        Init();
    }

    private void Start()
    {
        InputHandler_Start();
        
        LevelDicLoad();
        
        TeamLoad();
    }

    private void Update()
    {
        InputHandler_Update();
        
        UpdateCHActualDataPanel();
        
        TeleportationCooltimer();

        UpdateCHControlCoolFunc();
    }

    private void OnDrawGizmos()
    {
        //测试
        Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 50f, Color.red);
        Gizmos.DrawSphere(hitPos, 0.2f);
        Debug.DrawLine(this.transform.position, inputData.attackDir * 50f, Color.green);
    }

    private void LateUpdate()
    {
        CameraLookPosUpdate();
    }

    #endregion
    
    #region 外部访问字段

        /// <summary>
        /// 获取动画驱动模块
        /// </summary>
        public CharacterAnimatorDriver GetCHAnimatorDriver
        {
            get { return CHAnimatorDrivers[currentCHIndex]; }
        }

        /// <summary>
        /// 获取当前主控角色资源信息配置文件
        /// </summary>
        public CharacterAssetInforamtion GetCHAssetInfo
        {
            get { return CHAssetInfos[currentCHIndex]; }
        }
        
        /// <summary>
        /// 获取当前主控角色animator组件
        /// </summary>
        public Animator GetCHAnimator
        {
            get
            {
                if (loadCompleted)
                {
                    return CHAnimatorDrivers[currentCHIndex].animator;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取当前主控角色CharacterController组件
        /// </summary>
        public CharacterController GetCHCC
        {
            get
            {
                if (loadCompleted)
                {
                    return CHAnimatorDrivers[currentCHIndex].cc;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取当前主控角色的GameObject
        /// </summary>
        public GameObject GetCHGO
        {
            get
            {
                if (loadCompleted)
                {
                    return CHAnimatorDrivers[currentCHIndex].gameObject;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取当前角色的实时数据面板
        /// </summary>
        public UnitActualDataPanel GetCHActualDataPanel
        {
            get
            {
                if (loadCompleted)
                {
                    return CHActualDataPanels[currentCHIndex];
                }
                else
                {
                    return null;
                }
            }
            set{ CHActualDataPanels[currentCHIndex] = value; }
        }

        /// <summary>
        /// 根据信息索引得到实际面板
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public UnitActualDataPanel GetCHActualDataPanelByAssetInfo(CharacterAssetInforamtion info)
        {
            if (info != null && CHAssetInfos.Contains(info))
            {
                int index = CHAssetInfos.IndexOf(info);
                return CHActualDataPanels[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取增幅模块
        /// </summary>
        public BonusPanel GetCHBonusPanel
        {
            get { return bonusPanel; }
            set { bonusPanel = value; }
        }

        /// <summary>
        /// 获取瞬移技能当前的剩余CD
        /// </summary>
        public float GetTeleportationCooltimer
        {
            get { return teleportationCooltimer; }
        }

        #endregion
    
    #region 初始化(基本组件获取,面板显示加载)

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        playerInput = this.GetComponent<PlayerInput>();
        
        //面板显示加载
        PanelManager.Instance.PanelDisplay<InteractionPanel>("Interaction Panel", UILayer.Bot);
        PanelManager.Instance.PanelDisplay<EnergyPanel>("Energy Panel", UILayer.Bot);
        PanelManager.Instance.PanelDisplay<GameNoticePanel>("Game Notice Panel", UILayer.Top);
        PanelManager.Instance.PanelDisplay<SkillPanel>("Skill Panel", UILayer.Bot);
        PanelManager.Instance.PanelDisplay<CHStatePanel>("CH State Panel", UILayer.Bot);
        PanelManager.Instance.PanelDisplay<ToolbarPanel>("Toolbar Panel", UILayer.Bot);
        PanelManager.Instance.PanelDisplay<MapPanel>("Map Panel", UILayer.Bot);
        
        PanelManager.Instance.PanelDisplay<GameNoticePanel>("Game Notice Panel", UILayer.Top);
    }

    private void UnitActualDataPanelInit()
    {
        CHActualDataPanels.Clear();
        
        for (int i = 0; i < CHAssetInfos.Count; i++)
        {
            CHActualDataPanels.Add(new UnitActualDataPanel());
            CHActualDataPanels[i].level = levelDic.GetLevel(CHAssetInfos[i].assetID);
        }
    }

    private void CurHealthInit()
    {
        if(initHealth) return;
        
        for (int i = 0; i < CHAssetInfos.Count; i++)
        {
            CHActualDataPanels[i].curHealth = CHActualDataPanels[i].maxHealth;
        }
        
        initHealth = true;
    }

    private void BulletRemainInit()
    {
        if(bulletCapInit) return;
        
        for (int i = 0; i < CHAssetInfos.Count; i++)
        {
            CHActualDataPanels[i].bulletCount = CHAssetInfos[i].ammunitionCapacity;
        }
        
        bulletCapInit = true;
    }

    #endregion
    
    #region 摄像机机位控制

    private void CameraLookPosUpdate()
    {
        if (loadCompleted)
        {
            this.transform.localPosition = viewCenter;
            this.transform.localRotation = Quaternion.identity;
        }
    }

    #endregion

    #region 测试参数

    private Ray mouseRay;
    private Vector3 hitPos;

    #endregion

    #region 技能指示器

    public SectorIndicator indicator;

    #endregion

    #region 输入监听

    /// <summary>
    /// 输入键位监听
    /// </summary>
    private void InputHandler_Start()
    {
        //相机行为监听
        playerInput.actions["ViewSwitch"].performed += (context) =>
        {
            cameras[cameraIndex].SetActive(false);
            cameraIndex = (cameraIndex + 1) % cameras.Count;
            cameras[cameraIndex].SetActive(true);
        };
        
        //打开背包
        playerInput.actions["Bag"].performed += (context) =>
        {
            PanelManager.Instance.PanelDisplay<BagPanel>("Bag Panel", UILayer.Mid);
        };
        
        //切人键位
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            playerInput.actions[$"Team{index + 1}"].performed += (context) =>
            {
                if(!loadCompleted || inputData.jump || index > CHAssetInfos.Count - 1 || index == currentCHIndex || CHAnimatorDrivers[index].death) return;

                if (updateCHControlTimer < updateCHControlCooltime)
                {
                    PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Character switch is on cooldown");
                    return;
                }

                updateCHControlTimer = 0;
                
                previousCHIndex = currentCHIndex;
                currentCHIndex = index;
                
                UpdateControlCH();
            };
        }

        playerInput.actions["Burst"].performed += (context) =>
        {
            if (!loadCompleted || inputData.jump || inputData.burst) return;
            
            //判断能量是否充足释放爆发技
            if (PanelManager.Instance.GetPanel<EnergyPanel>("Energy Panel").EnergyAmpleBurst(GetCHAssetInfo.dataBase.energyStorageValue))
            {
                //生成技能指示器，目前仅支持扇形指示器
                indicator.SectorDisplay(GetCHAssetInfo.chSkillDataTable.burstAreaConfig.angle, GetCHAssetInfo.chSkillDataTable.burstAreaConfig.radius);
                //使用技能
                playerInput.actions["Burst"].canceled += UseBurst;

                Time.timeScale = 0.2f;
                inputData.indicating = true;
            }
            else
            {
                PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Energy Unample!");
            }
        };
        playerInput.actions["Talent"].performed += (context) =>
        {
            if (!loadCompleted || inputData.jump || inputData.burst) return;
            
            //判断
            if (GetCHActualDataPanel.talentCooltimer != 0) return;
            
            indicator.SectorDisplay(GetCHAssetInfo.chSkillDataTable.talentAreaConfig.angle, GetCHAssetInfo.chSkillDataTable.talentAreaConfig.radius);
            
            playerInput.actions["Talent"].canceled += UseTalent;
            
            Time.timeScale = 0.2f;
            inputData.indicating = true;
        };
        
        playerInput.actions["Reload"].performed += (context) =>
        {
            if (!loadCompleted || inputData.jump || inputData.burst || inputData.talent) return;

            inputData.reload = true;
        };
        playerInput.actions["Attack"].performed += (context) =>
        {
            if (!loadCompleted || inputData.jump || inputData.burst || inputData.talent || inputData.reload) return;

            if (GetCHActualDataPanel.bulletCount == 0)
            {
                PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Insufficient ammunition");
                return;
            }

            //记录鼠标点击方位
            Vector3 mouseScreenPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

            mouseRay = ray;
            
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.3f ,20f, mouseAttackRotateLayer);
            
            Vector3 worldPosition = hits[0].point;

            for (int i = 0; i < hits.Length; i++)
            {
                //遍历如果存在敌人单位则攻击朝向敌人单位，反之检查第一个射线触碰物体
                if(hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) worldPosition = hits[i].transform.position;
            }
            
            hitPos = worldPosition;
            
            inputData.attackDir = Vector3.ProjectOnPlane(worldPosition - GetCHGO.transform.position, Vector3.up);
            
            inputData.attack = true;
        };
        playerInput.actions["Jump"].performed += (context) =>
        {
            if (!loadCompleted || GetCHAssetInfo.specialAction == SpecialActionAcpability.Crouch || GetCHAssetInfo.specialAction == SpecialActionAcpability.Neither || !inputData.isAllowedCrouchOrJump || inputData.burst || inputData.talent || inputData.reload || inputData.attack) return;

            inputData.jump = true;
        };
        playerInput.actions["Crouch"].performed += (context) =>
        {
            if (!loadCompleted || GetCHAssetInfo.specialAction == SpecialActionAcpability.Jump || GetCHAssetInfo.specialAction == SpecialActionAcpability.Neither || !inputData.isAllowedCrouchOrJump || inputData.burst || inputData.talent || inputData.reload || inputData.attack) return;

            inputData.crouch = !inputData.crouch;
        };
    }

    private void UseBurst(InputAction.CallbackContext context)
    {
        GetCHGO.transform.rotation = indicator.transform.rotation;
        //设置参数
        inputData.burst = true;
        //减去能量
        PanelManager.Instance.GetPanel<EnergyPanel>("Energy Panel").UseEnergyBurst(GetCHAssetInfo.dataBase.energyStorageValue);
        //隐藏指示器
        indicator.SectorHide();
        
        Time.timeScale = 1;
        inputData.indicating = false;
        
        playerInput.actions["Burst"].canceled -= UseBurst;
    }
    
    private void UseTalent(InputAction.CallbackContext context)
    {
        GetCHGO.transform.rotation = indicator.transform.rotation;
        //设置参数
        inputData.talent = true;
        //减去能量
        GetCHActualDataPanel.talentCooltimer = GetCHAssetInfo.dataBase.talentCooltime;
        //隐藏指示器
        indicator.SectorHide();
        
        Time.timeScale = 1;
        inputData.indicating = false;
        
        playerInput.actions["Talent"].canceled -= UseTalent;
    }
    
    /// <summary>
    /// 输入键位读取
    /// </summary>
    private void InputHandler_Update()
    {
        if (playerInput.currentActionMap.name == "Gaming")
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                playerInput.currentActionMap.Disable();
                UpdateMouseTex(false);
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                playerInput.currentActionMap.Enable();
                UpdateMouseTex();
            }
        }

        if (loadCompleted)
        {
            if (inputData.indicating)
            {
                Vector3 mouseScreenPos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
                
                RaycastHit hit;
            
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAttackRotateLayer))
                {
                    Vector3 mouseGetDir = Vector3.ProjectOnPlane(hit.point - GetCHGO.transform.position, Vector3.up);
                    
                    indicator.transform.rotation = Quaternion.LookRotation(mouseGetDir, Vector3.up);
                    
                    Debug.DrawLine(indicator.transform.position, mouseGetDir * 3, Color.yellow);
                }
            }
            
            //Move
            Vector2 moveData = playerInput.actions["Move"].ReadValue<Vector2>();
            float horizontal = moveData.x;
            float vertical = moveData.y;
            Vector3 right = Camera.main.transform.right;
            Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            Vector3 moveDirection = (right * horizontal + forward * vertical).normalized;
            
            CHAnimatorDrivers[currentCHIndex].inputData = this.inputData;
            
            if(!inputData.jump) inputData.moveDirection = moveDirection;
        }

        if (inputDebuger)
        {
            
            Debug.Log("爆发技状态: " + inputData.burst);
            Debug.Log("天赋技状态: " + inputData.talent);
            Debug.Log("换弹状态: " + inputData.reload);
            Debug.Log("攻击状态: " + inputData.attack);
            Debug.Log("是否允许下蹲/跳跃: " + inputData.isAllowedCrouchOrJump);
            Debug.Log("蹲起状态: " + inputData.crouch);
            Debug.Log("跳跃状态: " + inputData.jump);
            Debug.Log("移动方向: " + inputData.moveDirection);
        }
    }

    #endregion

    #region 鼠标图标更新

    private void UpdateMouseTex(bool updateCHAttackTex = true)
    {
        if(updateCHAttackTex) Cursor.SetCursor(GetCHAssetInfo.mouseTexture, new Vector2(GetCHAssetInfo.mouseTexture.width / 2, GetCHAssetInfo.mouseTexture.height / 2), CursorMode.Auto);
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    #endregion

    #region 主控角色更新冷却

    private void UpdateCHControlCoolFunc()
    {
        updateCHControlTimer += Time.deltaTime;
        updateCHControlTimer = Mathf.Min(updateCHControlTimer, updateCHControlCooltime);
    }

    #endregion

    #region 等级字典加载

    private void LevelDicLoad()
    {
        // for (int i = 0; i < CHAssetInfos.Count; i++)
        // {
        //     levelDic.AddCH(CHAssetInfos[i].assetID);
        // }
        // levelDic.SaveLevelDic();
        
        //加载数据文件
        levelDic.LoadLevelDic();
    }

    #endregion

    #region 编队加载与更新

    public TeamConfiguration localTeamConfiguration = new TeamConfiguration();
    
    /// <summary>
    /// 编队加载
    /// </summary>
    private void TeamLoad()
    {
        // TeamConfiguration nowTeamConfiguration = new TeamConfiguration(CHAssetInfos);
        // nowTeamConfiguration.SaveTeamConfiguration();
        UpdateTeam();
    }

    public void UpdateTeam()
    {
        loadCompleted = false;
        
        StartCoroutine(localTeamConfiguration.LoadTeamConfiguration(TeamUpdate));
    }

    /// <summary>
    /// 编队更新
    /// </summary>
    /// <param name="newTeamConfiguration"></param>
    private async void TeamUpdate(List<CharacterAssetInforamtion> newTeamConfiguration)
    {
        if(TeamLoadDebuger) Debug.Log("编队信息更新中");

        if (newTeamConfiguration == null)
        {
            Debug.Log("新编队信息为空，无法更新");
            return;
        }
        
        if(TeamLoadDebuger) Debug.Log("清除原编队");
        this.transform.parent = null;
        //销毁模型并清空信息列表
        for(int i = 0; i < CHAnimatorDrivers.Count; i++) Destroy(CHAnimatorDrivers[i].gameObject);
        
        CHAssetInfos.Clear();
        CHAnimatorDrivers.Clear();
        
        if(TeamLoadDebuger) Debug.Log("新编队实例化中");
        //填充新配对信息并实例化模型
        for (int i = 0; i < newTeamConfiguration.Count; i++)
        {
            int index = i;
            CHAssetInfos.Add(newTeamConfiguration[index]);

            CHChipInfoDatas.Add(new CHChipInfoData() { topChipInfo = new ItemChipInfo(), botChipInfo = new ItemChipInfo() });
            AddressableManager.Instance.LoadAssetAsync<ItemChipInfo>(DataCenter.Instance.chipsDataDic.GetCHChipsData(newTeamConfiguration[i].assetID).topChipID,
                (info) =>
                {
                    CHChipInfoDatas[index].topChipInfo = info;
                });
            AddressableManager.Instance.LoadAssetAsync<ItemChipInfo>(DataCenter.Instance.chipsDataDic.GetCHChipsData(newTeamConfiguration[i].assetID).botChipID,
                (info) =>
                {
                    CHChipInfoDatas[index].botChipInfo = info;
                });
            
            GameObject go = Instantiate(newTeamConfiguration[index].modle);
            go.transform.position = this.transform.position;
            
            CharacterAnimatorDriver driver = go.GetComponent<CharacterAnimatorDriver>();
            driver.CHAssetInfo = CHAssetInfos[index];
            CHAnimatorDrivers.Add(driver);
            
            go.SetActive(false);
        }
        
        if (currentCHIndex >= CHAssetInfos.Count || previousCHIndex >= CHAssetInfos.Count)
        {
            currentCHIndex = 0;
            previousCHIndex = 0;
        }
        
        UpdateControlCH();
        
        UnitActualDataPanelInit();
        
        loadCompleted = true;
        
        initHealth = false;
    }

    /// <summary>
    /// 更新主控角色
    /// </summary>
    private void UpdateControlCH()
    {
        if(TeamLoadDebuger) Debug.Log("更新主控角色中");
        
        CHAnimatorDrivers[previousCHIndex].inputData.moveDirection = Vector3.zero;
        inputData = CHAnimatorDrivers[currentCHIndex].inputData;
        
        //隐藏老角色显示新角色
        CHAnimatorDrivers[previousCHIndex].animator.SetBool("Active", false);
        
        if(loadCompleted) this.transform.localPosition = Vector3.zero;

        if (!CHAnimatorDrivers[currentCHIndex].gameObject.activeInHierarchy)
        {
            CHAnimatorDrivers[currentCHIndex].transform.position = this.transform.position;
            CHAnimatorDrivers[currentCHIndex].transform.rotation = this.transform.rotation;
        }
        
        CHAnimatorDrivers[currentCHIndex].gameObject.SetActive(true);
        CHAnimatorDrivers[currentCHIndex].animator.SetBool("Active", true);
        
        //设置玩家模块位置
        this.transform.SetParent(CHAnimatorDrivers[currentCHIndex].gameObject.transform);
        this.transform.localPosition = viewCenter;

        UpdateMouseTex();
        
        EventCenter.Instance.SetEventTrigger("UpdateControlCH");
    }

    public void CurCHDeath()
    {
        previousCHIndex = currentCHIndex;
        currentCHIndex = (currentCHIndex + 1) % CHAssetInfos.Count;
        
        UpdateControlCH();
        
        CHAnimatorDrivers[previousCHIndex].gameObject.SetActive(false);
    }

    #endregion

    #region 实时数据面板更新

    private void UpdateCHActualDataPanel()
    {
        if(!loadCompleted) return;
        
        bonusPanel.OnUpdate();
        
        for (int i = 0; i < CHAssetInfos.Count; i++)
        {
            //基础数值实际面板计算
            //实际值 = 基础值 * （成长度 * 等级 + 1） * （ 1 + 加成 ）
            CHActualDataPanels[i] = GeneralDataHandler.CHActualPanelHandle(CHActualDataPanels[i], CHAssetInfos[i].dataBase, CHAssetInfos[i].dataGrowth, bonusPanel, CHChipInfoDatas[i], levelDic.GetLevel(CHAssetInfos[i].assetID));

            if (initHealth && CHActualDataPanels[i].curHealth == 0)
            {
                CHAnimatorDrivers[i].death = true;
            }
            
            //计算冷却
            CHActualDataPanels[i].talentCooltimer = (CHActualDataPanels[i].talentCooltimer - Time.deltaTime) < 0 ? 0 : CHActualDataPanels[i].talentCooltimer - Time.deltaTime;

            if (PanelManager.Instance.GetPanel<EnergyPanel>("Energy Panel") != null)
            {
                CHActualDataPanels[i].burstEnergyAmple = PanelManager.Instance.GetPanel<EnergyPanel>("Energy Panel").EnergyAmpleBurst(GetCHAssetInfo.dataBase.energyStorageValue);
            }
            
            CHActualDataPanels[i].curHealth = Mathf.Clamp(CHActualDataPanels[i].curHealth, 0, CHActualDataPanels[i].maxHealth);

            CHAnimatorDrivers[i].behaviorContext.actualData = CHActualDataPanels[i];
        }

        if (CHActualDataDebug)
        {
            Debug.Log("Attack:" + GetCHActualDataPanel.attack);
            Debug.Log("Defence:" + GetCHActualDataPanel.defence);
            Debug.Log("Speed:" + GetCHActualDataPanel.speed);
            Debug.Log("CriticalHitRate:" + GetCHActualDataPanel.criticalHitRate);
            Debug.Log("CriticalHitDamage:" + GetCHActualDataPanel.criticalHitDamage);
            Debug.Log("DamageBonus:" + GetCHActualDataPanel.damageBonus);
            Debug.Log("EnergyEfficiency:" + energyEfficiency);
        }

        CurHealthInit();

        BulletRemainInit();
    }

    #endregion

    #region 技能使用与冷却

    private void TeleportationCooltimer()
    {
        teleportationCooltimer = (teleportationCooltimer - Time.deltaTime) < 0 ? 0 : teleportationCooltimer - Time.deltaTime;
    }

    public void PlayerControlDisable()
    {
        playerInput.currentActionMap = inputAsset.FindActionMap("UI");
        UpdateMouseTex(false);
    }

    public void PlayerControlEnable()
    {
        playerInput.currentActionMap = inputAsset.FindActionMap("Gaming");
        UpdateMouseTex(true);
    }

    #endregion
}
