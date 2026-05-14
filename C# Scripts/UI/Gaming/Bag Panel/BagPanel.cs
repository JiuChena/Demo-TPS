using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BagPanel : PanelBase
{
    public Button btnClose;

    public TMP_Text moneyText;
    
    [Header("物品种类栏")] 
    private Animator previousAnimator;
    private Animator currentAnimator;
    public List<Animator> itemOptionAnimators;

    private SpriteAtlas atlas;
    private Queue<GameObject> bufferGOs = new Queue<GameObject>();
    private int count = 0;
    
    protected override void LoadInit()
    {
        previousAnimator = null;
        currentAnimator = itemOptionAnimators[0];
        
        LoadAtlas();
    }

    protected override void CompomentInit()
    {
        btnClose.onClick.AddListener((() =>
        {
            PanelManager.Instance.PanelHide("Bag Panel");
        }));
    }

    protected override void OnUpdate()
    {
        MouseClick();
        
        Debug.Log("pre:" + previousAnimator?.name);
        Debug.Log("cur:" + currentAnimator?.name);
        
        Debug.Log(bufferGOs.Count);
    }

    public void ReDisplay()
    {
        this.gameObject.SetActive(true);
        previousAnimator = null;
        
        UpdateBagPanel();
    }

    /// <summary>
    /// 图集加载
    /// </summary>
    private void LoadAtlas()
    {
        AddressableManager.Instance.LoadAssetAsync<SpriteAtlas>("Items", (atlas) =>
        {
            this.atlas = atlas;
            
            UpdateBagPanel();
        });
    }

    private void UpdateBagPanel()
    {
        StartCoroutine(RefreshBagPanel());
    }

    private IEnumerator RefreshBagPanel()
    {
        //更换物品栏种类时清理缓存
        if (previousAnimator == currentAnimator) yield break;
        
        StartCoroutine(ReleaseObjectsToPool());
        
        previousAnimator?.SetBool("Select", false);
        currentAnimator?.SetBool("Select", true);
        
        
        moneyText.text = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Money ,"Money").ToString() + "$";

        string itemTypeStr = currentAnimator.gameObject.name;
        
        RectTransform contentTrans = this.transform.Find("Center/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        foreach (KeyValuePair<ItemType, SerializableDictionary<string, int>> kvp1 in DataCenter.Instance.bagDataDic)
        {
            if (kvp1.Key.ToString() == itemTypeStr)
            {
                foreach (KeyValuePair<string, int> kvp2 in kvp1.Value)
                {
                    ObjectsPool.Instance.GetObjectFromPool("Bag_Store Content Item", contentTrans, (obj) =>
                    {
                        string itemName = kvp2.Key;
                        int itemAmount = kvp2.Value;
                        Image image = obj.transform.Find("Icon").GetComponent<Image>();
                        TMP_Text text = obj.transform.Find("Number").GetComponent<TMP_Text>();
                        obj.transform.Find("Mask").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        
                        obj.transform.localScale = Vector3.one;
                        obj.name = itemName;
                        
                        text.text = itemAmount.ToString();
                        bufferGOs.Enqueue(obj);
                        
                        image.sprite = atlas.GetSprite(itemName);
                    });
                    
                    count++;
                    if(count % 50 == 0) yield return null;
                }

                count = 0;
                break;
            }
        }
        
        contentTrans.sizeDelta = new Vector2(contentTrans.sizeDelta.x, 40 + (count / 8 + (count % 8 == 0 ? 0 : 1)) * 140);
    }

    private IEnumerator ReleaseObjectsToPool(UnityAction callback = null)
    {
        int count = 0;
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
            //释放资源，销毁面板
            AddressableManager.Instance.ReleaseResource<SpriteAtlas>("BagItems");
            DestroyPanel();
        }));
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

                if (topUI.tag == "UI_Bag_ItemTypeOption")
                {
                    string optionName = topUI.transform.parent.name;
                    
                    foreach (Animator animator in itemOptionAnimators)
                    {
                        if (animator.name == optionName)
                        {
                            previousAnimator = currentAnimator;
                            currentAnimator = animator;

                            UpdateBagPanel();
                            
                            break;
                        }
                    }
                }
                else if (topUI.tag == "UI_Bag_Store_Item")
                {
                    string itemName = topUI.transform.parent.name;
                    
                    //加载对应物品配置文件，打开详细信息面板，关闭面板时释放配置文件资源
                    PanelManager.Instance.PanelDisplay<BagItemMessagePanel>("Bag Item Message Panel", UILayer.Mid, (panel) =>
                    {
                        panel.PanelInit(currentAnimator.name, itemName, atlas.GetSprite(itemName));
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
}
