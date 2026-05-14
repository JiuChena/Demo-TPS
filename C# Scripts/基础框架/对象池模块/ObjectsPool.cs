using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectsPool : MonoBehaviour
{
    private static ObjectsPool instance;

    public static ObjectsPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "ObjectsPool";
                instance = obj.AddComponent<ObjectsPool>();
                DontDestroyOnLoad(obj);
            }
            
            return instance;
        }
    }

    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    
    private Queue<BufferReturnObjectInfo> bufferQueue = new Queue<BufferReturnObjectInfo>();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        int count = bufferQueue.Count;
        for (int i = 0; i < count; i++)
        {
            BufferReturnObjectInfo temp = bufferQueue.Dequeue();
            temp.delayTime -= Time.deltaTime;
            if (temp.delayTime <= 0)
                ReturnObjectToPool(temp.obj);
            else
                bufferQueue.Enqueue(temp);
        }
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="objectName">预制体名字</param>
    /// <param name="parent">要挂载的父物体</param>
    /// <param name="callback">加载完成后的回调函数</param>
    /// <returns>返回加载的对象</returns>
    public async Task<GameObject> GetObjectFromPool(string objectName, Transform parent = null, UnityAction<GameObject> callback = null)
    {
        GameObject obj = null;
        if (objectPool.ContainsKey(objectName) && objectPool[objectName].Count > 0)
        {
            obj = objectPool[objectName].Dequeue();
            obj.transform.SetParent(parent, true);
            callback?.Invoke(obj);
            obj.SetActive(true);
        }
        else
        {
            Task<AsyncOperationHandle<GameObject>> task = AddressableManager.Instance.LoadAssetAsync<GameObject>(objectName, (result) =>
            {
                obj = Instantiate(result);
                obj.SetActive(false);
                obj.name = objectName;
                obj.transform.SetParent(parent, true);
                callback?.Invoke(obj);
                obj.SetActive(true);
            });

            await task;
        }
        
        return obj;
    }
    
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="prefab">加载物体的预制体</param>
    /// <param name="parent">父对象</param>
    /// <param name="callback">加载完成的回调函数</param>
    /// <returns>返回加载的对象</returns>
    public async Task<GameObject> GetObjectFromPool(GameObject prefab, Transform parent = null, UnityAction<GameObject> callback = null)
    {
        GameObject obj = null;
        if (objectPool.ContainsKey(prefab.name) && objectPool[prefab.name].Count > 0)
        {
            obj = objectPool[prefab.name].Dequeue();
            obj.transform.SetParent(parent, true);
            callback?.Invoke(obj);
            obj.SetActive(true);
        }
        else
        {
            Task<AsyncOperationHandle<GameObject>> task = AddressableManager.Instance.LoadAssetAsync<GameObject>(prefab.name, (result) =>
            {
                obj = Instantiate(result);
                obj.SetActive(false);
                obj.name = prefab.name;
                obj.transform.SetParent(parent, true);
                callback?.Invoke(obj);
                obj.SetActive(true);
            });
            
            await task;
        }
        
        return obj;
    }
    
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="objectName">预制体名字</param>
    /// <param name="parent">要挂载的父物体</param>
    /// <param name="callback">加载完成后的回调函数</param>
    /// <returns>返回加载的对象</returns>
    public GameObject GetEmptyObjectFromPool(string objectName, Transform parent = null, UnityAction<GameObject> callback = null)
    {
        GameObject obj = null;
        if (objectPool.ContainsKey(objectName) && objectPool[objectName].Count > 0)
        {
            obj = objectPool[objectName].Dequeue();
            obj.transform.SetParent(parent, true);
            callback?.Invoke(obj);
            obj.SetActive(true);
        }
        else
        {
            obj = new GameObject(objectName);
            obj.transform.SetParent(parent, true);
            callback?.Invoke(obj);
            obj.SetActive(true);
        }
        
        return obj;
    }

    /// <summary>
    /// 返还对象到对象池
    /// </summary>
    /// <param name="obj">所返还的对象</param>
    public void ReturnObjectToPool(GameObject obj, UnityAction<GameObject> callback = null)
    {
        if(obj == null) return;
        
        callback?.Invoke(obj);
        
        if (objectPool.ContainsKey(obj.name))
        {
            objectPool[obj.name].Enqueue(obj);
        }
        else
        {
            objectPool.Add(obj.name, new Queue<GameObject>());
            objectPool[obj.name].Enqueue(obj);
        }
        
        obj.transform.SetParent(this.transform, true);
        obj.SetActive(false);
    }

    public void ReturnObjectToPool(GameObject obj, float delayTime)
    {
        bufferQueue.Enqueue(new BufferReturnObjectInfo(obj, delayTime));
    }
}

public class BufferReturnObjectInfo
{
    public GameObject obj;
    public float delayTime;

    public BufferReturnObjectInfo(GameObject obj, float delayTime)
    {
        this.obj = obj;
        this.delayTime = delayTime;
    }
}
