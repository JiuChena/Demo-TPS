using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentController : MonoBehaviour
{
    private ScrollRect scrollRect;
    public float cellHight;
    public float spaceHight;
    
    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        GridLayoutGroup layoutGroup = scrollRect.content.AddComponent<GridLayoutGroup>();
        layoutGroup.cellSize = new Vector2(scrollRect.content.rect.width, cellHight);
        layoutGroup.spacing = new Vector2(0, spaceHight);
    }

    private void Update()
    {
        int count = scrollRect.content.childCount;
        
        print(count);
        
        scrollRect.content.sizeDelta = new Vector2(0, count * (cellHight + spaceHight));
    }
}
