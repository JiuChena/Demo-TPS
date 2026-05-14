using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreItemMessagePanel : PanelBase
{
    public TMP_Text itemName;
    public Image itemIcon;
    public TMP_Text itemDescription;
    public TMP_Text bagAmount;
    public TMP_Text storeAmount;
    public TMP_InputField itemAmountInput;
    public Slider itemAmountSlider;
    public Transform itemSources;
    public Button close;
    public GameObject buy;

    private ItemInfoBase itemInfo;
    private UnityAction action;
    private ItemType itemType;
    private string assetName;
    private int buyAmount;
    private bool isUpdating = false;
    
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        close.onClick.AddListener(() =>
        {
            PanelManager.Instance.PanelHide("Store Item Message Panel");
        });
        
        buy.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            //购买物品逻辑  
            if (itemInfo.Buy(buyAmount))
            {
                PanelManager.Instance.PanelHide("Store Item Message Panel");
                
                TaskSystem.Instance.SetTaskTrigger("Store Task 1", buyAmount);
            }
        });
        
        //初始化最大数量以及滑动条最大数量
        itemAmountInput.onValueChanged.AddListener((value) =>
        {
            if (isUpdating) return; // 已在更新中，跳过

            int.TryParse(value, out int amount);
            
            if (amount <= 0) amount = 1;
            else if (amount > itemAmountSlider.maxValue) amount = (int)itemAmountSlider.maxValue;

            isUpdating = true;
            try
            {
                itemAmountInput.text = amount.ToString();
                itemAmountSlider.value = amount;
                buyAmount = amount;
            }
            finally
            {
                isUpdating = false;
            }
        });
        
        itemAmountSlider.onValueChanged.AddListener((value) =>
        {
            if (isUpdating) return; // 已在更新中，跳过

            int amount = Mathf.RoundToInt(value);
            isUpdating = true;
            try
            {
                itemAmountInput.text = amount.ToString();
                buyAmount = amount;
            }
            finally
            {
                isUpdating = false;
            }
        });
    }

    protected override void OnUpdate()
    {
        
    }
    
    public override void DisplayPanel()
    {
        
    }

    public override void HidePanel()
    {
        DataCenter.Instance.bagDataDic.SaveBagDataDic();
        
        action?.Invoke();
        
        PanelManager.Instance.GetPanel<StorePanel>("Store Panel")?.ReDisplay();
        
        DestroyPanel();
    }
    
    public void PanelInit(string itemType, string itemID, Sprite itemIcon)
    {
        switch (itemType)
        {
            case "Ammunition":
                this.itemType = ItemType.Ammunition;
                LoadItemMessage<ItemAmmunitionInfo>(itemID, itemIcon);
                break;
            case "Potion":
                this.itemType = ItemType.Potion;
                LoadItemMessage<ItemPotionInfo>(itemID, itemIcon);
                break;
            case "Prop":
                this.itemType = ItemType.Prop;
                LoadItemMessage<ItemPropInfo>(itemID, itemIcon);
                break;
            case "Chip":
                this.itemType = ItemType.Chip;
                LoadItemMessage<ItemChipInfo>(itemID, itemIcon);
                break;
            case "Character":
                this.itemType = ItemType.Character;
                LoadItemMessage<ItemCharacterInfo>(itemID, itemIcon);
                break;
        }
    }
    
    private void LoadItemMessage<T>(string itemID, Sprite itemIcon) where T : ItemInfoBase
    {
        AddressableManager.Instance.LoadAssetAsync<T>(itemID, (info) =>
        {
            assetName = info.name;
            this.itemName.text = info.itemName;
            this.itemIcon.sprite = itemIcon;
            switch (itemType)
            {
                case ItemType.Character:
                    this.itemIcon.color = Color.white;
                    this.bagAmount.text = (PlayerControlModule.Instance.levelDic.GetLevel(assetName) == 0 ? 0 : 1).ToString();
                    break;
                default:
                    this.itemIcon.color = Color.black;
                    this.bagAmount.text = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, itemID).ToString();
                    break;
            }
            this.itemDescription.text = info.itemDescription;
            this.storeAmount.text = DataCenter.Instance.storeData.GetStoreItemRemain(info.itemName).ToString();
            if(this.storeAmount.text == "-1") this.storeAmount.text = "*";
            itemAmountSlider.maxValue = storeAmount.text == "*" ? 9999 : int.Parse(storeAmount.text);
            foreach (string source in info.sources)
            {
                ObjectsPool.Instance.GetObjectFromPool("Item Source Item", itemSources, (obj) =>
                {
                    obj.transform.Find("Text").GetComponent<TMP_Text>().text = source;
                    obj.transform.localScale = Vector3.one;
                });
            }

            action += () =>
            {
                AddressableManager.Instance.ReleaseResource<T>(itemID);
            };
                    
            itemInfo = info;
        });
    }
}
