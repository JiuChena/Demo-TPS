using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarPanel : PanelBase
{
    public Button quitButton;
    public Button characterButton;
    public Button bagButton;
    public Button storeButton;
    public Button teamButton;
    public Button taskButton;
    
    [Header("退出选项")]
    public ExitOption exitOption;
    public string designedSceneName;
    
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        quitButton.onClick.AddListener(ExitFunction);
        characterButton.onClick.AddListener(DisplayCharacterPanel);
        bagButton.onClick.AddListener(DisplayBagPanel);
        storeButton.onClick.AddListener(DisplayStorePanel);
        teamButton.onClick.AddListener(DisplayTeamPanel);
        taskButton.onClick.AddListener(DisplayTaskPanel);
    }

    protected override void OnUpdate()
    {
        
    }

    private void ExitFunction()
    {
        switch (exitOption)
        {
            case ExitOption.ExitGame:
                //显示message面板并且将退出函数推送给message面板
                break;
            case ExitOption.ExitToDesignatedScene:
                //显示message面板并将切换指定场景的函数推送给message面板
                break;
        }
    }

    private void DisplayCharacterPanel()
    {
        PanelManager.Instance.PanelDisplay<CharacterPanel>("Character Panel", UILayer.Mid);
    }

    private void DisplayBagPanel()
    {
        PanelManager.Instance.PanelDisplay<BagPanel>("Bag Panel", UILayer.Mid);
    }

    private void DisplayStorePanel()
    {
        PanelManager.Instance.PanelDisplay<StorePanel>("Store Panel", UILayer.Mid);
    }
    
    private void DisplayTeamPanel()
    {
        PanelManager.Instance.PanelDisplay<TeamPanel>("Team Panel", UILayer.Mid);
    }

    private void DisplayTaskPanel()
    {
        PanelManager.Instance.PanelDisplay<TaskPanel>("Task Panel", UILayer.Mid);
    }
}
