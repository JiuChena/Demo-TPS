using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyGroups : MonoBehaviour
{
    [Header("地表位置")] public Transform groundSurface;
    public float triggerRadius = 5;
    public SerializableDictionary<EnemyAssetInformation, int> groups = new SerializableDictionary<EnemyAssetInformation, int>();
    
    private bool playerStayInEnemyArea = false;
    private bool displayEnemys = false;
    private float timer = 0f;
    
    private Queue<GameObject> enemys = new Queue<GameObject>();
    
    private SphereCollider collider;
    
    private void Start()
    {
        collider = GetComponent<SphereCollider>();
        
        collider.radius = triggerRadius;

        foreach (var item in groups)
        {
            LuaEnvManager.Instance.DoLua(item.Key.assetID + "Behavior");
        }
    }

    private void Update()
    {
        if (playerStayInEnemyArea && !displayEnemys)
        {
            //按照配置生成敌人
            foreach (var item in groups)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    //调用工厂方法
                    CreateEnemyUnit(item.Key);
                }
            }

            collider.radius = 10f;
            displayEnemys = true;
        }
        else if(!playerStayInEnemyArea && displayEnemys)
        {
            timer += Time.deltaTime;

            if (timer >= 10f)
            {
                //归还对象池
                for (int i = 0; i < groups.Count; i++)
                {
                    ObjectsPool.Instance.ReturnObjectToPool(enemys.Dequeue());
                }
                
                timer = 0f;
                displayEnemys = false;
                Debug.Log("玩家出界");
                collider.radius = 5f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) playerStayInEnemyArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) playerStayInEnemyArea = false;
    }

    public void CreateEnemyUnit(EnemyAssetInformation assetInfo, float createRange = 3)
    {
        ObjectsPool.Instance.GetObjectFromPool(assetInfo.assetID, null, (obj) =>
        {
            CharacterController collider = obj.GetComponent<CharacterController>();
            float height = collider.height;
            float radius = collider.radius;
            
            CreateEnemyFlag:
            
            Vector3 createPos = this.transform.position + new Vector3(Random.Range(-createRange, createRange), groundSurface.position.y + 0.2f, Random.Range(-createRange, createRange));

            if (Physics.SphereCast(createPos + new Vector3(0, radius, 0), radius + 0.1f, Vector3.up, out RaycastHit hit, height - radius * 2))
            {
                goto CreateEnemyFlag;
            }
            else
            {
                obj.transform.position = createPos;
                enemys.Enqueue(obj);
            }
        });
    }
}
