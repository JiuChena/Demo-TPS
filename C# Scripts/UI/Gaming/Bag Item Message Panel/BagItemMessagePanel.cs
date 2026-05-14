using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagItemMessagePanel : PanelBase
{
    public TMP_Text itemName;
    public Image itemIcon;
    public TMP_Text itemDescription;
    public TMP_Text itemAmount;
    public Transform itemSources;
    public Button close;
    public GameObject buy;
    public GameObject use;

    private string assetName;

    private UnityAction action;
    
    private ItemType itemType;
    private ItemInfoBase _itemInfo;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        close.onClick.AddListener(() =>
        {
            PanelManager.Instance.PanelHide("Bag Item Message Panel");
        });
        
        buy.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            //打开商店
            PanelManager.Instance.PanelDisplay<StorePanel>("Store Panel", UILayer.Mid);
            PanelManager.Instance.PanelHide("Bag Panel");
            PanelManager.Instance.PanelHide("Bag Item Message Panel");
        });
        
        use.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            //使用物品
            _itemInfo.Use();
            
            PanelManager.Instance.PanelHide("Bag Item Message Panel");
        });
    }

    protected override void OnUpdate()
    {
        
    }

    public void PanelInit(string itemType, string itemName, Sprite itemIcon)
    {
        switch (itemType)
        {
            case "Ammunition":
                this.itemType = ItemType.Ammunition;
                LoadItemMessage<ItemAmmunitionInfo>(itemName, itemIcon);
                break;
            case "Potion":
                this.itemType = ItemType.Potion;
                LoadItemMessage<ItemPotionInfo>(itemName, itemIcon);
                break;
            case "Prop":
                this.itemType = ItemType.Prop;
                LoadItemMessage<ItemPropInfo>(itemName, itemIcon);
                break;
            case "Chip":
                this.itemType = ItemType.Chip;
                LoadItemMessage<ItemAmmunitionInfo>(itemName, itemIcon);
                break;
        }
    }

    public override void DisplayPanel()
    {
        
    }

    public override void HidePanel()
    {
        DataCenter.Instance.bagDataDic.SaveBagDataDic();
        
        action?.Invoke();
        
        PanelManager.Instance.GetPanel<BagPanel>("Bag Panel")?.ReDisplay();
        
        DestroyPanel();
    }

    private void LoadItemMessage<T>(string itemName, Sprite itemIcon) where T : ItemInfoBase
    {
        AddressableManager.Instance.LoadAssetAsync<T>(itemName, (info) =>
        {
            assetName = info.name;
            this.itemName.text = info.name;
            this.itemIcon.sprite = itemIcon;
            this.itemDescription.text = info.itemDescription;
            this.itemAmount.text = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, itemName).ToString();
            foreach (string source in info.sources)
            {
                ObjectsPool.Instance.GetObjectFromPool("Item Source Item", itemSources, (obj) =>
                {
                    obj.transform.Find("Text").GetComponent<TMP_Text>().text = source;
                    obj.transform.localScale = Vector3.one;
                });
            }
                    
            if(!info.canBeUsed) use.SetActive(false);
            if(!info.canBeBought) buy.SetActive(false);

            action += () =>
            {
                AddressableManager.Instance.ReleaseResource<T>(itemName);
            };
                    
            _itemInfo = info;
        });
    }
}
