using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterSubpanel_Main : MonoBehaviour
{
    [Header("基础信息")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text SpecialActText;
    [Header("面板属性")]
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text defenceText;
    public TMP_Text speedText;
    public TMP_Text criticalRateText;
    public TMP_Text criticalDamageText;
    public TMP_Text damageBoostText;
    [Header("交互组件配置")]
    public GameObject dataPanel;
    public Button levelUpPanelClose;
    public Button levelUpPanelDisplayButton;
    public Button levelUpButton;
    public GameObject levelUpPanel;
    [Header("升级面板组件配置")]
    public TMP_Text costMoneyText;
    public TMP_Text costExpText;
}
