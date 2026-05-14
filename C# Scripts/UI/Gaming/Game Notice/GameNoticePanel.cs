using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameNoticePanel : PanelBase
{
    private TMP_Text text;
    private Queue<string> noticeQueue = new Queue<string>();
    [Tooltip("每条通知占用时间")] public float DisplayTime; 
    
    private float timer = 0;

    private bool firstDis = true;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        text = this.transform.Find("Content").GetComponent<TMP_Text>();
    }

    protected override void OnUpdate()
    {
        if (noticeQueue.Count > 0)
        {
            timer += Time.deltaTime;

            if (timer >= DisplayTime || firstDis)
            {
                timer = 0;
                
                text.text = noticeQueue.Dequeue();
                DisplayPanel();
            }
        }
        else
        {
            timer += Time.deltaTime;
            
            if (timer >= DisplayTime)
            {
                timer = 0;
                
                HidePanel();
            }
        }
    }

    public override void DisplayPanel()
    {
        animator.SetBool("Display", true);
        
        firstDis = false;
    }

    public override void HidePanel()
    {
        animator.SetBool("Display", false);
        
        firstDis = true;
    }

    public void PushNotice(string notice)
    {
        noticeQueue.Enqueue(notice);
    }
}
