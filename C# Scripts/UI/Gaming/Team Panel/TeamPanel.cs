using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TeamPanel : PanelBase
{
    public GameObject teamHCItem;
    
    public Transform selectedContent;
    public RectTransform unselectedContent;
    
    public Button closeButton;

    public SpriteAtlas atlas;
    
    private List<string> localTeamCHIDs = new List<string>();
    
    private Dictionary<string, GameObject> CHItems = new Dictionary<string, GameObject>();
    private PointerEventData eventData;
    
    private List<string> selectedCHIDs = new List<string>();
    
    protected override void LoadInit()
    {
        
    }

    
    protected override void CompomentInit()
    {
        closeButton.onClick.AddListener(() =>
        {
            if (selectedCHIDs.Count == 0)
            {
                PanelManager.Instance.GetPanel<GameNoticePanel>("Game Notice Panel").PushNotice("The formation cannot be empty!");
                return;
            }
            
            PanelManager.Instance.PanelHide("Team Panel");
            
            PlayerControlModule.Instance.localTeamConfiguration.TeamCHIDs.SaveCHIDsToFile(selectedCHIDs);
            
            PlayerControlModule.Instance.UpdateTeam();
        });
        
        eventData = new PointerEventData(EventSystem.current);
        
        ExistingTeamCHIDLoad();

        StartCoroutine(DisplayCHS());
    }

    protected override void OnUpdate()
    {
        unselectedContent.sizeDelta = new Vector2(0, (unselectedContent.childCount / 8 + 1) * 160 + 100);
    }

    private void ExistingTeamCHIDLoad()
    {
        localTeamCHIDs.Clear();
        //根据等级字典加载所有拥有角色，根据本地队伍字典加载本地队伍角色，遍历等级字典（排除已在队伍角色）加载角色头像角色等级
        for (int i = 0; i < PlayerControlModule.Instance.CHAssetInfos.Count; i++)
        {
            localTeamCHIDs.Add(PlayerControlModule.Instance.CHAssetInfos[i].assetID);
        }
    }

    private IEnumerator DisplayCHS()
    {
        int count = 0;
        
        foreach (KeyValuePair<string, int> CHID in PlayerControlModule.Instance.levelDic)
        {
            if (!localTeamCHIDs.Contains(CHID.Key))
            {
                string key = CHID.Key;
                int level = CHID.Value;
                
                ObjectsPool.Instance.GetObjectFromPool(teamHCItem, unselectedContent, (obj) =>
                {
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = atlas.GetSprite(key);
                    obj.transform.Find("ID").GetComponent<Text>().text = key;
                    obj.transform.Find("Level").GetComponent<TMP_Text>().text = "Lv." + level.ToString();
                    obj.GetComponent<Button>().onClick.AddListener(PushCHToSelected);
                    obj.transform.localScale = Vector3.one;
                    
                    CHItems.Add(key, obj);
                });
            }
            else
            {
                ObjectsPool.Instance.GetObjectFromPool(teamHCItem, selectedContent, (obj) =>
                {
                    string key = CHID.Key;
                    int level = CHID.Value;
                    
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = atlas.GetSprite(key);
                    obj.transform.Find("ID").GetComponent<Text>().text = key;
                    obj.transform.Find("Level").GetComponent<TMP_Text>().text = "Lv." + level.ToString();
                    obj.GetComponent<Button>().onClick.AddListener(PushCHToUnselected);
                    obj.transform.localScale = Vector3.one;
                    
                    selectedCHIDs.Add(key);
                    
                    CHItems.Add(key, obj);
                });
            }
            
            count++;
            
            if(count % 30 == 0) yield return null;
        }
    }

    private void PushCHToSelected()
    {
        //拿到所点击UI的角色ID，对该物体的点击事件进行切换，对该Item转移到非编队中
        string ID = GetClickCHID();
        
        CHItems[ID].GetComponent<Button>().onClick.RemoveListener(PushCHToSelected);
        CHItems[ID].GetComponent<Button>().onClick.AddListener(PushCHToUnselected);
        
        CHItems[ID].transform.parent = selectedContent;
        
        selectedCHIDs.Add(ID);
    }
    
    private void PushCHToUnselected()
    {
        //拿到所点击UI的角色ID，对该物体的点击事件进行切换，对该Item转移到非编队中
        string ID = GetClickCHID();
        
        if(ID == null || ID == "") return;
        
        CHItems[ID].GetComponent<Button>().onClick.RemoveListener(PushCHToUnselected);
        CHItems[ID].GetComponent<Button>().onClick.AddListener(PushCHToSelected);
        
        CHItems[ID].transform.parent = unselectedContent;
        
        selectedCHIDs.Remove(ID);
    }

    private string GetClickCHID()
    {
        // 1. 设置检测位置为当前鼠标位置
        eventData.position = Input.mousePosition;

        // 2. 清空旧结果
        List<RaycastResult> results = new List<RaycastResult>();

        // 3. 执行射线检测 (核心代码)
        // EventSystem.current.RaycastAll 会自动查找场景中所有的 GraphicRaycaster 并进行检测
        EventSystem.current.RaycastAll(eventData, results);

        // 4. 处理结果
        if (results.Count > 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.CompareTag("Team CH Item"))
                {
                    string ID = results[i].gameObject.transform.parent.Find("ID").GetComponent<Text>().text;
                    print($"点击到了ID为{ID}的角色");
                    return ID;
                }
            }
            
            return null;
        }
        else
        {
            Debug.Log("点击了空白处 (未检测到 UI)");

            return null;
        }
    }
    
    public override void DisplayPanel()
    {
        PlayerControlModule.Instance.PlayerControlDisable();
        Camera.main.GetComponent<GaussianBlur>().enabled = true;
    }
    
    public override void HidePanel()
    {
        PlayerControlModule.Instance.PlayerControlEnable();
        Camera.main.GetComponent<GaussianBlur>().enabled = false;
        
        StartCoroutine(ReleaseObjectsToPool(() =>
        {
            DestroyPanel();
        }));
    }

    private IEnumerator ReleaseObjectsToPool(UnityAction callback)
    {
        int count = 0;
        foreach (KeyValuePair<string, GameObject> item in CHItems)
        {
            ObjectsPool.Instance.ReturnObjectToPool(item.Value);
            
            count++;
            
            if(count % 30 == 0) yield return null;
        }
        
        callback?.Invoke();
    }
}
