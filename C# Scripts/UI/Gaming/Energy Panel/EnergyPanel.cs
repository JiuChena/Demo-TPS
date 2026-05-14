using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyPanel : PanelBase
{
    private TMP_Text text;
    
    private RectTransform energy;
    
    private float curEnergy = 0;
    
    public static float MAX_ENERGY_STORAGE = 10;

    private float energyImageWidth = 0;
    
    protected override void LoadInit()
    {
        
    }

    protected override void CompomentInit()
    {
        energy = this.transform.Find("Cur Energy").GetComponent<RectTransform>();
        
        text = this.transform.Find("Number/Number Text").GetComponent<TMP_Text>();

        energyImageWidth = energy.sizeDelta.x;
    }

    protected override void OnUpdate()
    {
        float energyEfficiency = PlayerControlModule.Instance.energyEfficiency;
        curEnergy = Mathf.Clamp(curEnergy + energyEfficiency * Time.deltaTime, 0, MAX_ENERGY_STORAGE);
        energy.sizeDelta = new Vector2(energyImageWidth * (curEnergy / MAX_ENERGY_STORAGE), energy.sizeDelta.y);
        
        text.text = ((int)curEnergy).ToString();
    }

    public bool UseEnergyBurst(float energyBurst)
    {
        if(energyBurst > curEnergy) return false;
        else
        {
            curEnergy -= energyBurst;
            return true;
        }
    }

    public bool EnergyAmpleBurst(float burstEnergy)
    {
        if(burstEnergy > curEnergy) return false;
        else return true;
    }
}
