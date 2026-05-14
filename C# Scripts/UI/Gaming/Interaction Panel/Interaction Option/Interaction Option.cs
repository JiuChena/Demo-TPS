using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionOption : MonoBehaviour
{
    public Animator animator;
    public TMP_Text interactionText;

    public void OptionInit(string text)
    {
        interactionText.text = text;
        OptionUnselect();
    }

    public void OptionSelect()
    {
        animator.SetBool("Selected", true);
    }

    public void OptionUnselect()
    {
        animator.SetBool("Selected", false);
    }
}
