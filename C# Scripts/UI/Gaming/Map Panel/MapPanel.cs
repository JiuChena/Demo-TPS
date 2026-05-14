using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : PanelBase
{
    public Transform mapCamera;
    public float defaultCameraHeight;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        
    }

    protected override void OnUpdate()
    {
        
    }

    private void LateUpdate()
    {
        MapCameraUpdate();
    }

    private void MapCameraUpdate()
    {
        mapCamera.position = PlayerControlModule.Instance.transform.position + Vector3.up * defaultCameraHeight;
    }
}
