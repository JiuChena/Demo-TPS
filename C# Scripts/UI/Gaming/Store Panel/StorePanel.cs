using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StorePanel : PanelBase
{
    public Button btnClose;
    public TMP_Text moneyText;
    
    [Header("物品种类栏")] 
    private Animator previousAnimator;
    private Animator currentAnimator;
    public List<Animator> itemOptionAnimators;

    private SpriteAtlas atlas;
    private Queue<GameObject> bufferGOs = new Queue<GameObject>();
    
    public StoreItemsConfiguration config;

    private int count;
    private RectTransform contentTrans;
    private bool updateStoreInfo = false;
    
    protected override void LoadInit()
    {
        updateStoreInfo = DataCenter.Instance.storeData.GetStoreSerialNumber == config.storeSerialNumber ? false : true;
        
        if(updateStoreInfo) DataCenter.Instance.storeData.UpdateData(config);
        
        Debug.Log(updateStoreInfo);
        
        previousAnimator = null;
        currentAnimator = itemOptionAnimators[0];
        
        LoadAtlas();
    }

    protected override void CompomentInit()
    {
        btnClose.onClick.AddListener(() =>
        {
            PanelManager.Instance.PanelHide("Store Panel");
        });
    }

    protected override void OnUpdate()
    {
        MouseClick();
    }
    
    /// <summary>
    /// 图集加载
    /// </summary>
    private void LoadAtlas()
    {
        AddressableManager.Instance.LoadAssetAsync<SpriteAtlas>("Items", (atlas) =>
        {
            this.atlas = atlas;
            
            UpdateStorePanel();
        });
    }
    
    private void MouseClick()
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

                if (topUI.tag == "UI_Store_ItemTypeOption")
                {
                    string optionName = topUI.transform.parent.name;
                    
                    foreach (Animator animator in itemOptionAnimators)
                    {
                        if (animator.name == optionName)
                        {
                            previousAnimator = currentAnimator;
                            currentAnimator = animator;

                            UpdateStorePanel();
                            
                            break;
                        }
                    }
                }
                else if (topUI.tag == "UI_Bag_Store_Item")
                {
                    if (topUI.transform.parent.Find("Mask").GetComponent<Image>().color.a != 0)
                    {
                        //提示商品售罄
                        Debug.Log("商品售罄");
                        return;
                    }
                    
                    string itemID = topUI.transform.parent.Find("AssetID").GetComponent<TMP_Text>().text;
                    
                    //加载对应物品配置文件，打开详细信息面板，关闭面板时释放配置文件资源
                    PanelManager.Instance.PanelDisplay<StoreItemMessagePanel>("Store Item Message Panel", UILayer.Mid, (panel) =>
                    {
                        panel.PanelInit(currentAnimator.name, itemID, atlas.GetSprite(itemID));
                        this.gameObject.SetActive(false);
                    });
                }
            }
            else
            {
                Debug.Log("点击到了空白区域（非 UI）");
            }
        }
    }

    private void UpdateStorePanel()
    {
        StartCoroutine(RefreshStorePanel());
    }
    
    private IEnumerator RefreshStorePanel()
    {
        //更换物品栏种类时清理缓存
        if (previousAnimator == currentAnimator) yield break;

        StartCoroutine(ReleaseObjectsToPool());
        
        previousAnimator?.SetBool("Select", false);
        currentAnimator?.SetBool("Select", true);
        
        
        moneyText.text = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money ,"Money").ToString() + "$";
        
        string itemTypeStr = currentAnimator.gameObject.name;
        
        contentTrans = this.transform.Find("Center/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        
        //加载本地商店余量信息，读取商店批次编号与当前配置批次是否相同，相同则按本地数据读取余量信息，不同则按配置文件读取余量信息

        switch (itemTypeStr)
        {
            case "Ammunition":
                StartCoroutine(ReadListConfig<ItemAmmunitionInfo>(config.canBeSoldAmmunitionInfo));
                break;
            case "Potion":
                StartCoroutine(ReadListConfig<ItemPotionInfo>(config.canBeSoldPotionInfo));
                break;
            case "Prop":
                StartCoroutine(ReadListConfig<ItemPropInfo>(config.canBeSoldPropInfo));
                break;
            case "Chip":
                StartCoroutine(ReadListConfig<ItemChipInfo>(config.canBeSoldChipInfo));
                break;
            case "Character":
                StartCoroutine(ReadListConfig<ItemCharacterInfo>(config.canBeSoldCharacterInfo));
                break;
        }
        
        contentTrans.sizeDelta = new Vector2(contentTrans.sizeDelta.x, 40 + (count / 8 + (count % 8 == 0 ? 0 : 1)) * 140);
    }

    private IEnumerator ReadListConfig<T>(List<StoreItemInfo<T>> config) where T : ItemInfoBase
    {
        foreach (StoreItemInfo<T> item in config)
        {
            ObjectsPool.Instance.GetObjectFromPool("Bag_Store Content Item", contentTrans, (obj) =>
            {
                string assetID = item.itemInfo.name;
                //itemName
                string itemName = item.itemInfo.itemName;
                //余量
                int itemRemain = DataCenter.Instance.storeData.GetStoreItemRemain(itemName);
                //价格
                float price = item.itemInfo.value;
                TMP_Text IDText = obj.transform.Find("AssetID").GetComponent<TMP_Text>();
                //图标组件
                Image image = obj.transform.Find("Icon").GetComponent<Image>();
                //价格组件
                TMP_Text numberTxet = obj.transform.Find("Number").GetComponent<TMP_Text>();
                //余量组件
                TMP_Text remainText = obj.transform.Find("Remain").GetComponent<TMP_Text>();
                
                obj.transform.localScale = Vector3.one;
                //改ID
                IDText.text = assetID;
                //改名称
                obj.name = itemName;
                //改图标
                image.sprite = atlas.GetSprite(assetID);
                if(currentAnimator.gameObject.name == "Character") image.color = Color.white;
                else image.color = Color.black;
                //改余量
                remainText.text = itemRemain.ToString();
                //查余量
                if(remainText.text == "-1") remainText.text = "*";
                if(remainText.text != "0") obj.transform.Find("Mask").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //改价格
                numberTxet.text = price.ToString() + " $";
                bufferGOs.Enqueue(obj);
            });
            
            count++;
            if(count % 50 == 0) yield return null;
        }
        
        count = 0;
    }

    private IEnumerator ReleaseObjectsToPool(UnityAction callback = null)
    {
        while (bufferGOs.Count > 0)
        {
            ObjectsPool.Instance.ReturnObjectToPool(bufferGOs.Dequeue(), (obj) =>
            {
                obj.name = "Bag_Store Content Item";
            });
            count++;
            if(count % 50 == 0) yield return null;
        }
        
        callback?.Invoke();
    }
    
    public void ReDisplay()
    {
        this.gameObject.SetActive(true);
        previousAnimator = null;
        
        UpdateStorePanel();
    }
    
    public override void DisplayPanel()
    {
        PlayerControlModule.Instance.PlayerControlDisable();
        Camera.main.GetComponent<GaussianBlur>().enabled = true;
    }

    public override void HidePanel()
    {
        PlayerControlModule.Instance.PlayerControlEnable();
        Camera.main.GetComponent<GaussianBlur>().enabled = false;
        
        this.gameObject.SetActive(true);
        
        StartCoroutine(ReleaseObjectsToPool(() =>
        {
            AddressableManager.Instance.ReleaseResource<SpriteAtlas>("Items");
            DestroyPanel();
        }));
    }
}
