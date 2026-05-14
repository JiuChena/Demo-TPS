using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L026 : PostEffectBase
{
    [Range(0, 1)] public float edgeOnly = 0;
    public Color edgeColor = Color.black;
    public Color backgroundColor = Color.white;
    
    public Shader edgeDetectShader;
    private Material edgeDetectMaterial;

    public Material material
    {
        get
        {
            edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
            return edgeDetectMaterial;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetFloat("_EdgeOnly", edgeOnly);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgroundColor);
            
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
