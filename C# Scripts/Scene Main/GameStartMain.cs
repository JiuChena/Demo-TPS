using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameStartMain : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> handle;
    
    private void Start()
    {
        PanelManager.Instance.PanelDisplay<GameStartPanel>("Game Start Panel", UILayer.Top);
        handle = AddressableManager.Instance.GetResourceHandle<GameObject>("Game Start Panel");
    }

    private void Update()
    {
        while (!handle.IsDone) return;

        if (Input.anyKeyDown)
        {
            //隐藏开始面板
            PanelManager.Instance.PanelHide("Game Start Panel");
            //显示加载面板
            PanelManager.Instance.PanelDisplay<LoadingPanel>("Loading Panel", UILayer.Top);
            //切换场景
            LoadSceneManager.Instance.LoadSceneAsync("Game Scene", (() =>
            {
                PanelManager.Instance.PanelHide("Loading Panel");
            }));
        }
    }
}
