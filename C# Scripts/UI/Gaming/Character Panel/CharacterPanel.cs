using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CharacterPanel : PanelBase
{
    private GameObject CHHallEnv;
    private Transform CHTrans;
    private GameObject CHModle;
    private Animator CHAnim;
    [Header("基础配置")]
    public Animator subpanelOptionAnimator;
    
    [Header("Main Panel")]
    public CharacterSubpanel_Main characterSubpanel_Main;
    public CharacterSubpanel_Skill characterSubpanel_Skill;
    public CharacterSubpanel_Chip characterSubpanel_Chip;
    public CharacterSubpanel_Information characterSubpanel_Information;
    
    [Header("组件配置")] 
    public RectTransform switchCHContent;
    public Button close;
    
    private Queue<GameObject> bufferGOs = new Queue<GameObject>();
    private Queue<string> bufferAssetIDs = new Queue<string>();
    private CHPanelAssetInfomation bufferCHAsset;
    private CHChipInfoData bufferCHChipInfoData = new CHChipInfoData();
    
    private Queue<string> bufferChipIDs = new Queue<string>();
    
    private bool initialized = false;
    private Animator preCHItemAnimator;
    private Animator curCHItemAnimator;

    private GameObject curSubpanelGO;
    
    private Animator skillItemAnimator;
    
    private int count = 0;
    
    int moneyCost, expCost, moneyBag, expBag;

    #region 继承方法（生命周期函数）的重写

    protected override void LoadInit()
    {
        PanelManager.Instance.PanelDisplay<LoadingPanel>("Loading Panel", UILayer.Top);
    }

    protected override void CompomentInit()
    {
        close.onClick.AddListener(() =>
        {
            PanelManager.Instance.PanelHide("Character Panel");
        });
        
        characterSubpanel_Main.levelUpPanelDisplayButton.onClick.AddListener(() =>
        {
            characterSubpanel_Main.dataPanel.SetActive(false);
            characterSubpanel_Main.levelUpPanel.SetActive(true);
            
            RefreshLevelUpPanel();
        });

        characterSubpanel_Main.levelUpButton.onClick.AddListener(() =>
        {
            if (PlayerControlModule.Instance.levelDic.ModifyCHLevel(bufferCHAsset.assetID, 1))
            {
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Money, "Money", moneyCost);
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Prop, "Experience", expCost);
                
                RefreshBasicInfoPanel();
                RefreshLevelUpPanel();
                
                CHAnim.SetTrigger("Cafe Idle");
            }
        });
    
        characterSubpanel_Main.levelUpPanelClose.onClick.AddListener(() =>
        {
            characterSubpanel_Main.levelUpPanel.SetActive(false);
            characterSubpanel_Main.dataPanel.SetActive(true);
        });
    }
    
    protected override void OnUpdate()
    {
        MouseClick();
    }

    #endregion

    #region 初始化

    private IEnumerator InitPanel()
    {
        //加载拥有角色的文件，根据文件加载角色头像Icon并且存入缓存队列，加载时直接配置在每一个item上放在切换角色的边栏，并且把他们都animator拿出来存到列表里
        
        //加载角色头像
        foreach (KeyValuePair<string, int> info in PlayerControlModule.Instance.levelDic)
        {
            string key = info.Key;
            
            ObjectsPool.Instance.GetObjectFromPool("Character Item", switchCHContent, (obj) =>
            {
                bufferGOs.Enqueue(obj);
                
                bufferAssetIDs.Enqueue(key);
                
                //ID赋值
                obj.transform.Find("AssetID").GetComponent<TMP_Text>().text = key;

                //加载头像
                AddressableManager.Instance.LoadAssetAsync<Sprite>(key, (sprite) =>
                {
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
                });
                
                obj.transform.localScale = Vector3.one;

                if (!initialized)
                {
                    ObjectsPool.Instance.GetObjectFromPool("Character Hall", null, (scene) =>
                    {
                        CHHallEnv = scene;
                        CHTrans = scene.transform.Find("Character");
                        
                        curCHItemAnimator = obj.GetComponent<Animator>();
                        AddressableManager.Instance.LoadAssetAsync<CHPanelAssetInfomation>(key, (asset) =>
                        {
                            bufferCHAsset = asset;
                            
                            //实例化角色模型
                            CHModle = Instantiate(bufferCHAsset.modle, CHTrans, false);
                            CHAnim = CHModle.GetComponent<Animator>();
                            
                            UpdateSubpanelOptionAnimator(subpanelOptionAnimator);
                        
                            RefreshPanel(subpanelOptionAnimator.name);
                        });
                    });
                    
                    initialized = true;
                }
            });
            
            count++;
            if (count % 50 == 0) yield return null;
        }

        switchCHContent.sizeDelta = new Vector2(switchCHContent.sizeDelta.x, 10 + 95 * switchCHContent.childCount);
        
        TimerEventManager.Instance.AddTimerEvent(1, () =>
        {
            PanelManager.Instance.PanelHide("Loading Panel");
        });
    }

    #endregion

    #region Refresh Panel

    private void RefreshPanel(string subpanelName)
    {
        preCHItemAnimator?.SetBool("Select", false);
        curCHItemAnimator?.SetBool("Select", true);
        
        //加载对应角色数据资源
        Task<AsyncOperationHandle<ItemChipInfo>>[] tasks = new Task<AsyncOperationHandle<ItemChipInfo>>[2];
        
        int index = 0;

        string key1 = DataCenter.Instance.chipsDataDic.GetCHChipsData(bufferCHAsset.assetID).topChipID;

        if (key1 != null && key1 != "")
        {
            Task<AsyncOperationHandle<ItemChipInfo>> task = AddressableManager.Instance.LoadAssetAsync<ItemChipInfo>(key1, (info) =>
            {
                bufferCHChipInfoData.topChipInfo = info;
            });
            
            bufferChipIDs.Enqueue(key1);
            tasks[index] = task;
            index++;
        }
        else
        {
            bufferCHChipInfoData.topChipInfo = new ItemChipInfo();
        }
        
        string key2 = DataCenter.Instance.chipsDataDic.GetCHChipsData(bufferCHAsset.assetID).botChipID;

        if (key2 != null && key2 != "")
        {
            Task<AsyncOperationHandle<ItemChipInfo>> task = AddressableManager.Instance.LoadAssetAsync<ItemChipInfo>(key2, (info) =>
            {
                bufferCHChipInfoData.botChipInfo = info;
            });
            
            bufferChipIDs.Enqueue(key2);
            tasks[index] = task;
            index++;
        }
        else
        {
            bufferCHChipInfoData.botChipInfo = new ItemChipInfo();
        }

        if (index != 0)
        {
            Task.WaitAll(tasks);
        }
        
        switch (subpanelName)
        {
            case "Main":
                curSubpanelGO?.SetActive(false);
                curSubpanelGO = characterSubpanel_Main.gameObject;
                curSubpanelGO.SetActive(true);
                
                RefreshBasicInfoPanel();
                
                RefreshLevelUpPanel();

                UnitActualDataPanel actualPanel = new UnitActualDataPanel();
                
                int level = PlayerControlModule.Instance.levelDic.GetLevel(bufferCHAsset.assetID);
                
                actualPanel = GeneralDataHandler.CHActualPanelHandle(actualPanel, bufferCHAsset.dataBase, bufferCHAsset.dataGrowth, new BonusPanel(), bufferCHChipInfoData, level);
                
                characterSubpanel_Main.healthText.text = actualPanel.maxHealth.ToString();
                characterSubpanel_Main.attackText.text = actualPanel.attack.ToString();
                characterSubpanel_Main.defenceText.text = actualPanel.defence.ToString();
                characterSubpanel_Main.speedText.text = actualPanel.speed.ToString();
                characterSubpanel_Main.criticalRateText.text = actualPanel.criticalHitRate.ToString();
                characterSubpanel_Main.criticalDamageText.text = actualPanel.criticalHitDamage.ToString();
                characterSubpanel_Main.damageBoostText.text = actualPanel.damageBonus.ToString();
                
                break;
            case "Skill":
                curSubpanelGO?.SetActive(false);
                curSubpanelGO = characterSubpanel_Skill.gameObject;
                curSubpanelGO.SetActive(true);
                
                
                
                break;
            case "Chip":
                curSubpanelGO?.SetActive(false);
                curSubpanelGO = characterSubpanel_Chip.gameObject;
                curSubpanelGO.SetActive(true);
                break;
            case "Information":
                curSubpanelGO?.SetActive(false);
                curSubpanelGO = characterSubpanel_Information.gameObject;
                curSubpanelGO.SetActive(true);
                break;
        }
    }

    #endregion

    #region MianPanel Refresh

    private void RefreshBasicInfoPanel()
    {
        characterSubpanel_Main.nameText.text = bufferCHAsset.assetID.Substring(0, 6);
        characterSubpanel_Main.levelText.text = "Lv." + PlayerControlModule.Instance.levelDic.GetLevel(bufferCHAsset.assetID).ToString();
        characterSubpanel_Main.SpecialActText.text = bufferCHAsset.specialAction.ToString();
        characterSubpanel_Main.iconImage.sprite = AddressableManager.Instance.GetResource<Sprite>(bufferCHAsset.assetID);
    }
    
    private void RefreshLevelUpPropData()
    {
        moneyCost = bufferCHAsset.dataGrowth.moneyCostFactorForCHNextLevel * PlayerControlModule.Instance.levelDic.GetLevel(bufferCHAsset.assetID);
        expCost = bufferCHAsset.dataGrowth.experienceItemCostFactorForCHNextLevel * PlayerControlModule.Instance.levelDic.GetLevel(bufferCHAsset.assetID);
        
        moneyBag = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money, "Money");
        expBag = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Prop, "Experience");
    }
    
    private void RefreshLevelUpPanel()
    {
        RefreshLevelUpPropData();
            
        characterSubpanel_Main.costMoneyText.text = moneyCost.ToString() + " / " + moneyBag.ToString();
        characterSubpanel_Main.costExpText.text = expCost.ToString() + " / " + expBag.ToString();

        if (moneyCost > moneyBag)
        {
            characterSubpanel_Main.costMoneyText.color = Color.red;
            characterSubpanel_Main.levelUpButton.interactable = false;
        }
        else
        {
            characterSubpanel_Main.costMoneyText.color = Color.green;
            characterSubpanel_Main.levelUpButton.interactable = true;
        }

        if (expCost > expBag)
        {
            characterSubpanel_Main.costExpText.color = Color.red;
            characterSubpanel_Main.levelUpButton.interactable = false;
        }
        else
        {
            characterSubpanel_Main.costExpText.color = Color.green;
        }
    }

    #endregion

    #region SkillPanel Refresh

    private void RefreshSkillPanel(Animator skillItemAnimator)
    {
        this.skillItemAnimator?.SetBool("Select", false);
        skillItemAnimator.SetBool("Select", true);
        this.skillItemAnimator = skillItemAnimator;

        switch (skillItemAnimator.name)
        {
            case "Normal Attack":
                break;
            case "Talent":
                break;
            case "Burst":
                break;
        }
    }

    #endregion

    #region MouseClick

    private async void MouseClick()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonUp(0))
        {
            // 创建一个 PointerEventData 对象，代表一次“指针事件”
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition; // 设置为当前鼠标位置

            // 存储所有被射线击中的 UI 元素（按渲染顺序从上到下）
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // 遍历结果（results[0] 是最上层的 UI）
            if (results.Count > 0)
            {
                GameObject topUI = results[0].gameObject;
                if (topUI.tag == "Character Item")
                {
                    Animator touchUIAnimator = topUI.transform.parent.GetComponent<Animator>();
                    
                    if(touchUIAnimator == curCHItemAnimator) return;
                    
                    preCHItemAnimator = curCHItemAnimator;
                    curCHItemAnimator = touchUIAnimator;
                    
                    ReleaseBufferAsset();
                    
                    //先找到是哪个角色，先把角色配置资源加载出来
                    string assetID = topUI.transform.parent.Find("AssetID").GetComponent<TMP_Text>().text;
                    
                    Task<AsyncOperationHandle<CHPanelAssetInfomation>> CHAssetInfoTask = AddressableManager.Instance.LoadAssetAsync<CHPanelAssetInfomation>(assetID, (asset) =>
                    {
                        bufferCHAsset = asset;
                        
                        //实例化角色模型
                        CHModle = Instantiate(bufferCHAsset.modle, CHTrans, false);
                        CHAnim = CHModle.GetComponent<Animator>();
                        
                        RefreshPanel(subpanelOptionAnimator.name);
                    });
                }
                else if (topUI.tag == "UI_Character Panel_Subpanel")
                {
                    Animator optionAnimator = topUI.transform.parent.GetComponent<Animator>();
                    UpdateSubpanelOptionAnimator(optionAnimator);

                    RefreshPanel(subpanelOptionAnimator.name);
                }
                else if (topUI.tag == "Character Skill Item")
                {
                    Animator skillItemAnimator = topUI.transform.parent.GetComponent<Animator>();
                    
                    RefreshSkillPanel(skillItemAnimator);
                }
            }
            else
            {
                Debug.Log("点击到了空白区域（非 UI）");
            }
        }
    }

    #endregion

    #region 副面板动画更新

    private void UpdateSubpanelOptionAnimator(Animator animator)
    {
        subpanelOptionAnimator.SetBool("Select", false);
        subpanelOptionAnimator = animator;
        subpanelOptionAnimator.SetBool("Select", true);
    }

    #endregion

    #region 继承方法重写

    public override void DisplayPanel()
    {
        PlayerControlModule.Instance.PlayerControlDisable();
        PanelManager.Instance.HideLayer(UILayer.Bot);

        StartCoroutine(InitPanel());
    }

    public override void HidePanel()
    {
        PlayerControlModule.Instance.PlayerControlEnable();
        PanelManager.Instance.DisplayLayer(UILayer.Bot);
        
        PanelManager.Instance.PanelDisplay<LoadingPanel>("Loading Panel", UILayer.Top);

        ReleaseBufferAsset();
        StartCoroutine(ReleaseGeneralAsset());
    }

    #endregion

    #region 资源释放

    private void ReleaseBufferAsset()
    {
        //释放角色的芯片配置文件资源
        if (bufferChipIDs.Count > 0)
        {
            string key = bufferChipIDs.Dequeue();
            AddressableManager.Instance.ReleaseResource<ItemChipInfo>(key);
            Debug.Log("释放了" + key);
        }
        
        //释放角色配置资源
        if (bufferCHAsset != null)
        {
            string key = bufferCHAsset.assetID;
            AddressableManager.Instance.ReleaseResource<CHPanelAssetInfomation>(bufferCHAsset.assetID);
            Debug.Log("释放了" + key);
        }
        
        //销毁角色模型
        if(CHModle != null) Destroy(CHModle);
    }

    private IEnumerator ReleaseGeneralAsset()
    {
        count = 0;

        while (bufferGOs.Count > 0)
        {
            ObjectsPool.Instance.ReturnObjectToPool(bufferGOs.Dequeue());
            
            count++;
            if(count % 50 == 0) yield return null;
        }

        while (bufferAssetIDs.Count > 0)
        {
            AddressableManager.Instance.ReleaseResource<Sprite>(bufferAssetIDs.Dequeue());
            
            count++;
            if(count % 50 == 0) yield return null;
        }
        
        if(CHHallEnv != null) ObjectsPool.Instance.ReturnObjectToPool(CHHallEnv);
        
        TimerEventManager.Instance.AddTimerEvent(1, () =>
        {
            PanelManager.Instance.PanelHide("Loading Panel");
        });
        
        DestroyPanel(0.5f);
    }

    #endregion
}
