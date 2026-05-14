using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    //拿到对应的技能信息
    private Queue<SkillEffect> triggerSkillEffects = new Queue<SkillEffect>();
    private GameObject owner;
    
    private List<GameObject> targets = new List<GameObject>();

    private void Update()
    {
        PreToCurRayCast();
    }
    
    public void Init(List<SkillEffect> skillEffects, GameObject owner)
    {
        this.owner = owner;
        
        for (int i = 0; i < skillEffects.Count; i++)
        {
            switch (skillEffects[i].triggerMode)
            {
                //即时触发效果
                case SkillEffectTriggerMode.InstantTrigger:
                    SkillEffectExecute.Execute(skillEffects[i], targets, owner);
                    break;
                //命中触发效果
                case SkillEffectTriggerMode.OnHitTrigger:
                    triggerSkillEffects.Enqueue(skillEffects[i]);
                    break;
                //即时持续触发
                //命中持续触发
            }
        }
    }

    private Vector3 prePos, curPos;
    private SkillEffect tempSkillEffect;

    //射线命中检测
    private void PreToCurRayCast()
    {
        prePos = curPos;
        curPos = this.transform.position;
        
        RaycastHit[] hits = Physics.RaycastAll(prePos, curPos - prePos, Vector3.Distance(prePos, curPos), PlayerControlModule.Instance.mouseAttackRotateLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && !hits[i].collider.isTrigger && !hits[i].collider.CompareTag("Player"))
            {
                targets.Add(hits[i].collider.gameObject);
                
                //命中目标了，但是不知道是什么,直接轮询之前存储的命中触发技能效果
                while (triggerSkillEffects.Count > 0)
                {
                    tempSkillEffect = triggerSkillEffects.Dequeue();
                    SkillEffectExecute.Execute(tempSkillEffect, targets, owner);
                    HitEffectCreate(hits[i], tempSkillEffect);
                }
                
                break;
            }
        }
    }
    
    //区域命中检测
    
    private void HitEffectCreate(RaycastHit hit, SkillEffect skillEffect)
    {
        //比较打击标签，直接生成对应标签的特效即可
        
        LayerMask hitObjLayer = hit.collider.gameObject.layer;

        if (skillEffect.casterEffectPrefab != null)
            ObjectsPool.Instance.GetObjectFromPool(skillEffect.casterEffectPrefab, owner.transform, (obj) =>
            {
                obj.transform.position = hit.point;
                obj.transform.up = hit.normal;
                ObjectsPool.Instance.ReturnObjectToPool(obj, 2);
            });

        foreach (LayerMask layer in skillEffect.skillHitEffects.Keys)
        {
            if ((layer.value & (1 << hitObjLayer)) != 0)
            {
                ObjectsPool.Instance.GetObjectFromPool(skillEffect.skillHitEffects[layer].hitEffect, hit.transform, (obj) =>
                {
                    obj.transform.position = hit.point;
                    obj.transform.up = hit.normal;
                    ObjectsPool.Instance.ReturnObjectToPool(obj, 2);
                });
            
                AudioManager.Instance.SetAudio(skillEffect.skillHitEffects[layer].hitSound, hit.collider.gameObject);
                
                break;
            }
        }
        
        ObjectsPool.Instance.ReturnObjectToPool(this.gameObject);
    }

    private void OnDisable()
    {
        targets.Clear();
        owner = null;
    }
}
