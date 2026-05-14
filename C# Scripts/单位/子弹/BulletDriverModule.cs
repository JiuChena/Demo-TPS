using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class BulletDriverModule : MonoBehaviour
{
    private UnitActualDataPanel attackerDataPanel;
    private float attackerSkillFactor;
    
    private float residenceTime;
    
    public LayerMask layerMask;
    
    private float bulletSpeed;
    
    private bool initialized = false;
    
    private bool afterHitDestory = false;

    private float retuenInsDelayTime = 0;
    
    public SerializableDictionary<LayerMask, HitInforamtion> hitEffects = new SerializableDictionary<LayerMask, HitInforamtion>();

    private void Update()
    {
        if (initialized)
        {
            this.transform.Translate(this.transform.forward * Time.deltaTime * bulletSpeed, Space.World);
        
            residenceTime += Time.deltaTime;

            if (residenceTime >= 3f)
            {
                residenceTime = 0;
                ObjectsPool.Instance.ReturnObjectToPool(this.gameObject);
            }
            
            BulletRayCast();
        }
    }

    public void Init(float bulletSpeed, UnitActualDataPanel attackerDataPanel, string attackername, bool afterHitDestory = false, float delayTime = 0)
    {
        this.bulletSpeed = bulletSpeed;
        this.attackerDataPanel = attackerDataPanel;
        
        previousPosition = this.transform.position;
        currentPosition = this.transform.position;

        _attackHitEnemy = LuaEnvManager.Instance.Global.Get<AttackHitEnemyDel>($"{attackername}_AttackHit");
        _attackHitPlayer = LuaEnvManager.Instance.Global.Get<AttackHitPlayerDel>($"{attackername}_AttackHitPlayer");
        
        initialized = true;
        this.afterHitDestory = afterHitDestory;
        retuenInsDelayTime = delayTime;
    }

    [CSharpCallLua]
    public delegate void AttackHitEnemyDel(UnitActualDataPanel attacker, UnitActualDataPanel defencer, GameObject target);
    [CSharpCallLua]
    public delegate void AttackHitPlayerDel(UnitActualDataPanel attacker, UnitActualDataPanel defencer, GameObject target);

    private AttackHitEnemyDel _attackHitEnemy;
    private AttackHitPlayerDel _attackHitPlayer;
    
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    //检测射线是否碰撞到物体，有则检查层级，效果什么的都显示在这个子弹物体上，直接造成伤害即可
    private void BulletRayCast()
    {
        previousPosition = currentPosition;
        currentPosition = this.transform.position;
        
        Debug.DrawRay(previousPosition, (currentPosition - previousPosition).normalized * Vector3.Distance(previousPosition, currentPosition), Color.green);
        
        //开始进行检测
        if (Physics.Raycast(previousPosition, currentPosition - previousPosition, out RaycastHit hit, Vector3.Distance(previousPosition, currentPosition), layerMask, QueryTriggerInteraction.Ignore))
        {
            //对这个检测到的层级进行检查，并且按照配置好的字典生成对应的特效
            foreach (KeyValuePair<LayerMask, HitInforamtion> hitInfo in hitEffects)
            {
                if ((hitInfo.Key.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    ObjectsPool.Instance.GetObjectFromPool(hitInfo.Value.hitEffect, null, (obj) =>
                    {
                        obj.transform.position = hit.point;
                        obj.transform.up = hit.normal;
                        ObjectsPool.Instance.ReturnObjectToPool(obj, 2);
                    });
                    
                    AudioManager.Instance.SetAudio(hitInfo.Value.hitSound, hit.collider.gameObject);

                    EnemyAnimatorDriver driver = hit.collider.GetComponent<EnemyAnimatorDriver>();
                    if (driver != null)
                    {
                        Debug.Log("打中的是Enemy");
                        _attackHitEnemy(attackerDataPanel, driver.actualData, hit.collider.gameObject);
                        
                        if(!afterHitDestory) ObjectsPool.Instance.ReturnObjectToPool(this.gameObject);
                        else ObjectsPool.Instance.ReturnObjectToPool(this.gameObject, retuenInsDelayTime);
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Debug.Log("打中的是Player");
                        _attackHitPlayer(attackerDataPanel, PlayerControlModule.Instance.GetCHActualDataPanel, hit.collider.gameObject);
                        
                        if(!afterHitDestory) ObjectsPool.Instance.ReturnObjectToPool(this.gameObject);
                        else ObjectsPool.Instance.ReturnObjectToPool(this.gameObject, retuenInsDelayTime);
                    }
                    else
                    {
                        Debug.Log("打中其他物体");
                    }
                    
                    break;
                }
            }
        }
    }
}

[Serializable]
public class HitInforamtion
{
    public GameObject hitEffect;
    public AudioClip hitSound;
}
