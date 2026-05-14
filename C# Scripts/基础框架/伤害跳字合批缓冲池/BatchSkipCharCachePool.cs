using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BatchSkipCharCachePool : MonoBehaviour
{
    private static BatchSkipCharCachePool instance;

    public static BatchSkipCharCachePool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("BatchSkipCharCachePool");
                instance = go.AddComponent<BatchSkipCharCachePool>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    [Header("参数")] public int batchSkipCharCount = 5;
    public float batchSkipCharTime = 0.2f;
    public float minSkipCharSize = 1;
    public float maxSkipCharSize = 1;
    public float skipCharScaleSpeed = 1;
    public float skipCharDropSpeed = 1;

    private Dictionary<GameObject, TargetSkipCharCacheInfo> cachePool =
        new Dictionary<GameObject, TargetSkipCharCacheInfo>();

    private List<TextMesh> skipChars = new List<TextMesh>();

    private void Start()
    {

    }

    private void Update()
    {
        CheckCachePool();

        SkipCharsHandler();
    }

    public void PushSkipChar(GameObject target, float damage)
    {
        if (!cachePool.ContainsKey(target)) cachePool.Add(target, new TargetSkipCharCacheInfo());

        cachePool[target].damageQueue.Enqueue(damage);
    }
    
    private void CheckCachePool()
    {
        foreach (var target in cachePool.Keys.ToList())
        {
            if (cachePool[target].damageQueue.Count > batchSkipCharCount)
            {
                //调用跳字显示
                SkipChar(target, cachePool[target].damageQueue);

                cachePool.Remove(target);
            }
            else
            {
                cachePool[target].timer += Time.deltaTime;

                if (cachePool[target].timer >= batchSkipCharTime)
                {
                    //调用跳字显示
                    SkipChar(target, cachePool[target].damageQueue);

                    cachePool.Remove(target);
                }
            }
        }
    }

    private void SkipChar(GameObject target, Queue<float> numsQueue)
    {
        Transform agent = target.transform.Find("SkipCharAgent");
        if (agent != null)
        {
            //然后从对象池拿跳字伤害，
            ObjectsPool.Instance.GetObjectFromPool("SkipChar", agent, (skipChar) =>
            {
                TextMesh mesh = skipChar.GetComponent<TextMesh>();
                mesh.text = numsQueue.Dequeue().ToString();
                mesh.transform.localPosition = new Vector3(0, 0.8f, 0);
                mesh.transform.localRotation = Quaternion.identity;
                mesh.transform.localScale = Vector3.one;
                mesh.characterSize = 0.02f;

                skipChars.Add(mesh);
            });
        }
        else
        {
            ObjectsPool.Instance.GetObjectFromPool("SkipCharAgent", target.transform, (agent) =>
            {
                agent.transform.localPosition = Vector3.zero;
                
                for(int i = 0; i < numsQueue.Count; i++)
                {
                    //然后从对象池拿跳字伤害，
                    ObjectsPool.Instance.GetObjectFromPool("SkipChar", agent.transform, (skipChar) =>
                    {
                        TextMesh mesh = skipChar.GetComponent<TextMesh>();
                        mesh.text = ((int)numsQueue.Dequeue()).ToString();
                        mesh.transform.localPosition = new Vector3(Random.Range(-0.3f, 0.3f), 0.8f, Random.Range(0.1f, 0.3f));
                        mesh.transform.localRotation = Quaternion.identity;
                        mesh.transform.localScale = Vector3.one;
                        mesh.characterSize = 0.02f;

                        skipChars.Add(mesh);
                    });
                }
            });
        }
    }
    
    private void SkipCharsHandler()
    {
        for (int i = 0; i < skipChars.Count; i++)
        {
            skipChars[i].transform.localRotation = Camera.main.transform.rotation;
            skipChars[i].characterSize = Mathf.Lerp(skipChars[i].characterSize, minSkipCharSize / 1000, skipCharScaleSpeed * Time.deltaTime);
            skipChars[i].transform.localPosition -= new Vector3(0, skipCharDropSpeed * Time.deltaTime, 0);

            if (skipChars[i].characterSize <= (minSkipCharSize + 1) / 1000)
            {
                ObjectsPool.Instance.ReturnObjectToPool(skipChars[i].gameObject);
                skipChars.Remove(skipChars[i]);
                i--;
            }
            
            Debug.Log("跳字");
        }
    }
}

public class TargetSkipCharCacheInfo
{
    public Queue<float> damageQueue = new Queue<float>();
    public float timer = 0;
}
