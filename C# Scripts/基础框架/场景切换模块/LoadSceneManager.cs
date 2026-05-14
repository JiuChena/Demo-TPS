using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadSceneManager
{
    private static LoadSceneManager instance = new LoadSceneManager();
    public static LoadSceneManager Instance => instance;

    /// <summary>
    /// 同步加载场景
    /// </summary>
    public void LoadScene(string sceneName, UnityAction callback = null)
    {
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    /// <summary>
    /// 异步加载场景，通过事件中心广播 "LoadSceneProgress" 传递进度
    /// </summary>
    public void LoadSceneAsync(string sceneName, UnityAction callback = null)
    {
        PublicMono.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, callback));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, UnityAction callback = null)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            EventCenter.Instance.SetEventTrigger("LoadSceneProgress", asyncOperation.progress);
            yield return asyncOperation.progress;
        }
        callback?.Invoke();
    }
}
