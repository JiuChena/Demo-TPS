using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class CHStatePanel : PanelBase
{
    [Header("血量显示")]
    public RectTransform curHealthBar;
    public TMP_Text curHealthText;
    public TMP_Text maxHealthText;

    [Header("子弹显示")] 
    public TMP_Text curBullets;
    public TMP_Text bagBullets;

    private float maxWidth;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        maxWidth = curHealthBar.sizeDelta.x;
    }

    protected override void OnUpdate()
    {
        if (PlayerControlModule.Instance.loadCompleted)
        {
            float curHealth = (int)PlayerControlModule.Instance.GetCHActualDataPanel.curHealth;
            float maxHealth = (int)PlayerControlModule.Instance.GetCHActualDataPanel.maxHealth;
            curHealthText.text = curHealth.ToString();
            maxHealthText.text = "/ " + maxHealth.ToString();
        
            curHealthBar.sizeDelta = new Vector2(maxWidth * (curHealth / maxHealth), curHealthBar.sizeDelta.y);

            curBullets.text = PlayerControlModule.Instance.GetCHActualDataPanel.bulletCount.ToString();
            
            int count = 0;
            switch (PlayerControlModule.Instance.GetCHAssetInfo.WeaponType)
            { 
                case WeaponType.Pistol:
                    //9mm
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-9mm");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();
                    break;
                case WeaponType.SubmachineGun:
                    //9mm
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-9mm");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();                
                    break;
                case WeaponType.Rifle:
                    //7.62mm
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-7.62mm");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();                   
                    break;
                case WeaponType.Shotgun:
                    //12号
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-12 Gauge");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();
                    break;
                case WeaponType.MachineGun:
                    //7.62
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-7.62mm");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();
                    break;
                case WeaponType.Howitzer:
                    //40mm
                    count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-40mm");
                    bagBullets.text = count > 999 ? "999+" : count.ToString();                
                    break;
            }
        }
    }
}

