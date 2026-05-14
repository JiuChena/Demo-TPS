using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class AddressableManager
{
    //能存handle，能存result
    private static readonly AddressableManager instance = new AddressableManager();
    public static AddressableManager Instance => instance;
    
    private Dictionary<string, ResourceContainer> resources = new Dictionary<string, ResourceContainer>();
    private Dictionary<string, byte[]> luaCache = new Dictionary<string, byte[]>();
    
    /// <summary>
    /// 异步加载指定资源
    /// </summary>
    /// <param name="key">资源ID（不建议使用标签，如需使用自行扩展）</param>
    /// <param name="callback">资源加载成功的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public async Task<AsyncOperationHandle<T>> LoadAssetAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;
        
        if (!resources.ContainsKey(keyLine))
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            
            resources.Add(keyLine, new ResourceContainer(handle));
            
            handle.Completed += (handle) =>
            {
                callback?.Invoke(handle.Result);
            };
            
            resources[keyLine].ResourceReferenced();
            
            return handle;
        }
        else
        {
            await resources[keyLine].handle.Task;
            
            Debug.Log(key + $"({typeof(T).Name}) is already loaded");
            
            callback?.Invoke(resources[keyLine].handle.Convert<T>().Result);
            
            resources[keyLine].ResourceReferenced();
            
            return resources[keyLine].handle.Convert<T>();
        }
    }

    /// <summary>
    /// 获取资源容器
    /// </summary>
    /// <param name="key">资源ID</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public ResourceContainer GetResourceContainer<T>(string key) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;

        if (!resources.ContainsKey(keyLine))
        {
            return null;
        }
        else
        {
            return resources[keyLine];
        }
    }
    
    /// <summary>
    /// 获取资源加载句柄
    /// </summary>
    /// <param name="key">资源ID</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public AsyncOperationHandle<T> GetResourceHandle<T>(string key) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;

        if (!resources.ContainsKey(keyLine))
        {
            Debug.Log(key + $"({typeof(T).Name}) is not loaded");
            return default(AsyncOperationHandle<T>);
        }
        else
        {
            return resources[keyLine].handle.Convert<T>();
        }
    }

    /// <summary>
    /// 获取资源加载状态
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AsyncOperationStatus GetResourceStatus<T>(string key)
    {
        string keyLine = typeof(T).Name + "_" + key;

        if (resources.ContainsKey(keyLine))
        {
            return resources[keyLine].handle.Status;
        }
        else
        {
            return AsyncOperationStatus.None;
        }
    }

    /// <summary>
    /// 获取已加载的资源
    /// </summary>
    /// <param name="key">资源ID</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public T GetResource<T>(string key) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;

        if (!resources.ContainsKey(keyLine))
        {
            Debug.Log(key + $"({typeof(T).Name}) is not loaded");
            return null;
        }
        else
        {
            resources[keyLine].ResourceReferenced();
            return resources[keyLine].handle.Result as T;
        }
    }

    /// <summary>
    /// 获取已加载的Lua文件的字节数组
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public byte[] GetLuaBytes(string filePath)
    {
        if (luaCache.ContainsKey(filePath))
        {
            return luaCache[filePath];
        }
        else
        {
            Debug.LogWarning($"路径为：{filePath}的文件未加载至缓存中！");
            return null;
        }
    }

    /// <summary>
    /// 资源是否存在？
    /// </summary>
    /// <param name="key">资源ID</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public bool GetResourceExist<T>(string key) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;
        
        if (resources.ContainsKey(keyLine)) return true;
        else return false;
    }

    /// <summary>
    /// 释放指定的单个资源
    /// </summary>
    /// <param name="key"></param>
    public void ReleaseResource<T>(string key) where T : Object
    {
        string keyLine = typeof(T).Name + "_" + key;
        
        if (resources.ContainsKey(keyLine))
        {
            if(resources[keyLine].ResourceReleased()) resources.Remove(keyLine);
            Debug.Log(keyLine + " is released once");
        }
        else
        {
            Debug.Log(keyLine + " is not loaded");
        }
    }

    /// <summary>
    /// 释放指定的一批资源（同类型）
    /// </summary>
    /// <param name="keys">资源ID列表</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void ReleaseResources<T>(List<string> keys) where T : Object
    {
        string resType = typeof(T).Name;
        
        for (int i = 0; i < keys.Count; i++)
        {
            string keyLine = resType + "_" + keys[i];
            
            if (resources.ContainsKey(keyLine))
            {
                if(resources[keyLine].ResourceReleased()) resources.Remove(keyLine);
                Debug.Log(keyLine + " is released");
            }
            else
            {
                Debug.Log(keyLine + " is not loaded");
            }
        }
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void ReleaseAllResources()
    {
        foreach(KeyValuePair<string, ResourceContainer> pair in resources) pair.Value.handle.Release();
        resources.Clear();
        Debug.Log("All resources have released");
    }
}

public class ResourceContainer
{
    public AsyncOperationHandle handle;
    public int count;

    public ResourceContainer(AsyncOperationHandle handle)
    {
        this.handle = handle;
        this.count = 0;
    }

    public void ResourceReferenced()
    {
        this.count += 1;
    }

    public bool ResourceReleased()
    {
        this.count -= 1;
        
        if (count == 0)
        {
            this.handle.Release();
            Debug.Log("资源计数归零，资源已经释放！");
            return true;
        }
        else return false;
    }
}