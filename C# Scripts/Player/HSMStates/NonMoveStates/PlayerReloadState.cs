using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerReloadState : PLayerNonMoveState
{
    public PlayerReloadState(HSM hsm) : base(hsm)
    {
        
    }

    public override void OnEnter()
    {
        hsm.CHDriver.animator.SetBool("Reload", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (hsm.CHDriver.inputData.burst || hsm.CHDriver.inputData.talent)
        {
            hsm.SwitchState<PlayerSkillState>();
        }
        
        if (!hsm.CHDriver.inputData.reload)
        {
            if (hsm.CHDriver.inputData.attack)
            {
                hsm.SwitchState<PlayerAttackState>();
            }
            else if (hsm.CHDriver.inputData.jump || hsm.CHDriver.inputData.moveDirection != Vector3.zero)
            {
                hsm.SwitchState<PlayerMoveState>();
            }
            else if (hsm.CHDriver.inputData.crouch || hsm.CHDriver.inputData.moveDirection == Vector3.zero)
            {
                hsm.SwitchState<PlayerIdleState>();
            }
        }
    }

    public override void OnExit()
    {
        Reload();
        
        hsm.CHDriver.animator.SetBool("Reload", false);
    }

    private void Reload()
    {
        int capacity = hsm.CHDriver.CHAssetInfo.ammunitionCapacity - hsm.CHDriver.behaviorContext.actualData.bulletCount;
        int count = 0;
        switch (PlayerControlModule.Instance.GetCHAssetInfo.WeaponType)
        { 
            case WeaponType.Pistol:
                //9mm
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-9mm");
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-9mm", Mathf.Min(capacity, count));
                break;
            case WeaponType.SubmachineGun:
                //9mm
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-9mm");
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-9mm", Mathf.Min(capacity, count));
                break;
            case WeaponType.Rifle:
                //7.62mm
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-7.62mm");    
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-7.62mm", Mathf.Min(capacity, count));
                break;
            case WeaponType.Shotgun:
                //12号
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-12 Gauge");
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-12 Gauge", Mathf.Min(capacity, count));
                break;
            case WeaponType.MachineGun:
                //7.62
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-7.62mm");
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-7.62mm", Mathf.Min(capacity, count));
                break;
            case WeaponType.Howitzer:
                //40mm
                count = DataCenter.Instance.bagDataDic.GetItemAmount(ItemType.Ammunition, "Ammo-40mm");         
                if(!CheckRemain(count)) return;
                DataCenter.Instance.bagDataDic.RemoveItemFromBag(ItemType.Ammunition, "Ammo-40mm", Mathf.Min(capacity, count));
                break;
        }
        
        int reloadCount = capacity > count ? count : capacity;


        hsm.CHDriver.behaviorContext.actualData.bulletCount += reloadCount;
    }

    private bool CheckRemain(int count)
    {
        if (count == 0)
        {
            PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("Not enough bullets remaining");
            return false;
        }
        else
        {
            return true;
        }
    }
}
