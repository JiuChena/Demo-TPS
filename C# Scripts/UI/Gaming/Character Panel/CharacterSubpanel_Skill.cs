using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSubpanel_Skill : MonoBehaviour
{
    public Animator normalAttack;
    public Animator talent;
    public Animator burst;

    public Image attackIcon;
    public Image talentIcon;
    public Image burstIcon;

    public TMP_Text normalAttackLevel;
    public TMP_Text talentLevel;
    public TMP_Text burstLevel;

    public Button attackLevelUpButton;
    public Button talentLevelUpButton;
    public Button burstLevelUpButton;
}
