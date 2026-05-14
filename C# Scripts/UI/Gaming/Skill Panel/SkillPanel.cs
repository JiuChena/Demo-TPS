using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SkillPanel : PanelBase
{
    private CharacterAssetInforamtion CHAssetInfo;
    
    public SpriteAtlas atlas;
    public GameObject buffItem;
    
    private Dictionary<BonusType, Queue<GameObject>> buffItems = new Dictionary<BonusType, Queue<GameObject>>();

    #region UI

    //瞬移
    public Image teleportationCooltimeMask;
    public Image teleportationImage;
    //Talent
    public Image talentCooltimeMask;
    public Image talentImage;
    //Burst
    public Image burstCooltimeMask;
    public Image burstImage;
    //Buff
    public Transform buffContent;

    #endregion
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        EventCenter.Instance.AddEventListener("UpdateControlCH", UpdatePanelBind);
    }

    protected override void OnUpdate()
    {
        UpdateSkillCooltime();
    }

    private void UpdatePanelBind()
    {
        //更新当前角色信息
        CHAssetInfo = PlayerControlModule.Instance.GetCHAssetInfo;

        talentImage.sprite = CHAssetInfo.CHTalentSprite;
        burstImage.sprite = CHAssetInfo.CHBurstSprite;
    }

    private void UpdateSkillCooltime()
    {
        if(!PlayerControlModule.Instance.loadCompleted) return;
        
        teleportationCooltimeMask.fillAmount = PlayerControlModule.Instance.GetTeleportationCooltimer / PlayerControlModule.Instance.teleportationCooltime;

        talentCooltimeMask.fillAmount = PlayerControlModule.Instance.GetCHActualDataPanel.talentCooltimer / PlayerControlModule.Instance.GetCHAssetInfo.dataBase.talentCooltime;

        burstCooltimeMask.fillAmount = PlayerControlModule.Instance.GetCHActualDataPanel.burstEnergyAmple ? 0 : 1;
    }
    
    public void AddBonus(BonusType bonusType)
    {
        ObjectsPool.Instance.GetObjectFromPool(buffItem, buffContent, (obj) =>
        {
            if(!buffItems.ContainsKey(bonusType)) buffItems.Add(bonusType, new Queue<GameObject>());
            buffItems[bonusType].Enqueue(obj);
            obj.transform.Find("Icon").GetComponent<Image>().sprite = atlas.GetSprite(bonusType.ToString());

            obj.transform.localScale = Vector3.one;
        });
    }

    public void RemoveBonus(BonusType bonusType)
    {
        ObjectsPool.Instance.ReturnObjectToPool(buffItems[bonusType].Dequeue());
    }
}

public enum BonusType
{
    Health,
    Attack,
    Defence,
    Speed,
    CriticalHitRate,
    CriticalHitDamage,
    Damage,
    EnergyEfficiency,
}
