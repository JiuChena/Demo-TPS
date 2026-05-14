using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UILayer
{
    Bot,
    Mid,
    Top,
    System,
}

public class PanelManager
{
    private static PanelManager instance = new PanelManager();
    public static PanelManager Instance => instance;
    
    private Dictionary<string, PanelBase> panelsDic = new Dictionary<string, PanelBase>();
    
    public Transform RectTrans_Canvas;
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public PanelManager()
    {
        AddressableManager.Instance.LoadAssetAsync<GameObject>("Canvas", (result) =>
        {
            GameObject obj = GameObject.Instantiate(result);
            
            RectTrans_Canvas = obj.transform;
            GameObject.DontDestroyOnLoad(obj);

            bot = RectTrans_Canvas.Find("Bot");
            mid = RectTrans_Canvas.Find("Mid");
            top = RectTrans_Canvas.Find("Top");
            system = RectTrans_Canvas.Find("System");
        });
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="layer"></param>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public async void PanelDisplay<T>(string panelName, UILayer layer, UnityAction<T> callback = null) where T : PanelBase
    {
        if (panelsDic.ContainsKey(panelName)) return;
        else panelsDic.Add(panelName, null);

        await AddressableManager.Instance.GetResourceHandle<GameObject>("Canvas").Task;
        
        AddressableManager.Instance.LoadAssetAsync<GameObject>(panelName, (result) =>
        {
            GameObject obj = GameObject.Instantiate(result);
            
            //找到根对象，设置在哪一层
            Transform trans_Root = system;
            switch (layer)
            {
                case UILayer.Bot:
                    trans_Root = bot;
                    break;
                case UILayer.Mid:
                    trans_Root = mid;
                    break;
                case UILayer.Top:
                    trans_Root = top;
                    break;
            }
            
            obj.transform.SetParent(trans_Root, false);
            
            T panelScript = obj.GetComponent<T>();
            panelsDic[panelName] = panelScript;
        
            panelScript.DisplayPanel();
        
            callback?.Invoke(panelScript);
        });
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelName">面板名字</param>
    /// <param name="target">目标对象</param>
    /// <param name="callback">有参回调函数</param>
    /// <typeparam name="T">参数类型</typeparam>
    public void PanelDisplay<T>(string panelName, Transform target, UnityAction<T> callback = null) where T : PanelBase
    {
        if (panelsDic.ContainsKey(panelName)) return;
        panelsDic.Add(panelName, null);

        AddressableManager.Instance.LoadAssetAsync<GameObject>(panelName, (prefab) =>
        {
            GameObject obj = GameObject.Instantiate(prefab);

            obj.transform.SetParent(target, false);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            T panelScript = obj.GetComponent<T>();
            panelsDic[panelName] = panelScript;

            panelScript.DisplayPanel();

            callback?.Invoke(panelScript);
        });
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName">面板名字(场景)</param>
    /// <param name="callback">无参回调函数</param>
    public void PanelHide(string panelName, UnityAction callback = null)
    {
        if (panelsDic.ContainsKey(panelName))
        {
            panelsDic[panelName].HidePanel();
            
            panelsDic.Remove(panelName);
            
            AddressableManager.Instance.ReleaseResource<GameObject>(panelName);
        }
        else return;
        
        callback?.Invoke();
    }

    /// <summary>
    /// 获取场景上已存在的面板(挂载的脚本)
    /// </summary>
    /// <param name="panelName">面板名字</param>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <returns></returns>
    public T GetPanel<T>(string panelName) where T : PanelBase
    {
        if(panelsDic.ContainsKey(panelName)) return panelsDic[panelName] as T;
        else return null;
    }

    /// <summary>
    /// 更改Canvas渲染模式
    /// </summary>
    /// <param name="renderMode"></param>
    public void ChangeCanvasRenderMode(RenderMode renderMode)
    {
        RectTrans_Canvas.GetComponent<Canvas>().renderMode = renderMode;
    }
    
    /// <summary>
    /// 显示指定UI层
    /// </summary>
    /// <param name="layer"></param>
    public void DisplayLayer(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Bot:
                bot.gameObject.SetActive(true);
                break;
            case UILayer.Mid:
                mid.gameObject.SetActive(true);
                break;
            case UILayer.Top:
                top.gameObject.SetActive(true);
                break;
            case UILayer.System:
                system.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 隐藏指定UI层
    /// </summary>
    /// <param name="layer"></param>
    public void HideLayer(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Bot:
                bot.gameObject.SetActive(false);
                break;
            case UILayer.Mid:
                mid.gameObject.SetActive(false);
                break;
            case UILayer.Top:
                top.gameObject.SetActive(false);
                break;
            case UILayer.System:
                system.gameObject.SetActive(false);
                break;
        }
    }
}


