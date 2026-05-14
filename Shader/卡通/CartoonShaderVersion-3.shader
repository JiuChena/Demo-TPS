Shader "CartoonShader/Character Shader/Version_3"
{
    Properties
    {
        _MainTex ("主纹理图", 2D) = "white" {}
        _Mask("阴影遮罩", 2D) = "black"{}
        _SpecularTex("高光贴图", 2D) = "white"{}
        _SpecularLightScale("高光反射强度", Range(0,2)) = 1
        _DiffuseColor("漫反射颜色混合", Color) = (1,1,1,1)
        _DiffuseLightScale("漫反射光照强度", Range(0,5)) = 0.5
        _Emission("自发光颜色", Color) = (1,1,1,1)
        _EmissionScale("自发光强度", Range(0,1)) = 0.5
        
        // 描边属性
        _OutlineColor("描边颜色", Color) = (0,0,0,1)
        _OutlineWidth("描边宽度(mm)", Range(0, 10)) = 0.005
        _OutlineIntensity ("描边强度", Range(0.0, 5.0)) = 1.0
        
        // 卡通属性
        _Steps("色阶阶数", Range(1,500)) = 3
        _ToonEffect("卡通渲染效果", Range(0,1)) = 0.5
        
        // 透视属性
        _XRayColor("遮挡透视颜色", Color) = (1,0,0,1) // 默认红色
        _XRayEnhancement("透视颜色强度", Range(1, 10)) = 1
        
        // 【修复】补充缺失的 Rim 属性
        _RimColor("边缘光颜色", Color) = (1,1,1,1)
        _RimPower("边缘光强度", Range(0.1, 5)) = 2.0
    }

    SubShader
    {
        // 【修复 1】移除 SubShader 层级的 RenderQueue，避免污染所有 Pass
        // 只保留 RenderType 用于分类
        Tags { "RenderType"="Opaque" "Queue" = "Geometry+1" } 
        LOD 100
        
        // ----------------------------------------------------------------
        // 1. 透视通道 (X-Ray Pass)
        // ----------------------------------------------------------------
        Pass
        {
            Name "XRayPass"
            Tags { 
                "LightMode" = "Always" 
                "Queue" = "Transparent+1" 
                "ForceNoShadowcasting" = "True" 
                "IgnoreProjector" = "True" 
            }
            
            Cull Front              // 只渲染背面
            ZWrite Off              // 不写入深度
            ZTest Greater           // 只渲染被遮挡的部分
            Blend SrcAlpha OneMinusSrcAlpha // 【修复 2】添加透明混合
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ENABLE_XRAY // 只有开启此关键词才编译
            
            #include "UnityCG.cginc"

            // 【修复 3】类型匹配：Color 对应 float4
            float4 _XRayColor;
            float _XRayEnhancement;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 计算最终颜色 (RGB * 强度, Alpha 保持原样或根据强度调整)
                // 这里假设 _XRayColor.a 是基础透明度
                #ifdef _ENABLE_XRAY
                // 只有当 Keyword 被启用时，才执行这里的代码
                    return float4(_XRayColor.rgb * _XRayEnhancement, 1);
                #else
                    // 如果没启用，必须丢弃像素或返回透明，否则 Pass 依然会画东西（虽然可能是黑的或透明的，但深度测试可能依然通过）
                    discard;
                    return float4(0,0,0,1);
                #endif
            }
            ENDCG
        }

        // ----------------------------------------------------------------
        // 2. 描边通道 (Outline Pass)
        // ----------------------------------------------------------------
        Pass
        {
            // 建议放在 Geometry+1 或 Transparent-1，确保在主物体之后，其他透明物体之前
            Tags { "Queue" = "Geometry+1" "LightMode"="Always" }
            
            Cull Front 
            ZWrite On 
            Blend SrcAlpha OneMinusSrcAlpha 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 viewDir = _WorldSpaceCameraPos - worldPos;
                
                // 法线外扩
                float3 outlineOffset = worldNormal * _OutlineWidth / 1000.0;
                float4 outlineVertex = v.vertex + float4(outlineOffset, 0.0);
                
                o.pos = UnityObjectToClipPos(outlineVertex);
                o.viewDir = viewDir;
                o.worldNormal = worldNormal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDirNorm = normalize(i.viewDir);
                float3 worldNormalNorm = normalize(i.worldNormal);
                float dotProduct = dot(worldNormalNorm, viewDirNorm);
                float edgeFactor = abs(dotProduct);
                float outlineStrength = 1.0 - edgeFactor;
                
                // 简单的阈值处理，让描边更清晰，避免整个背面都变色
                // 如果希望渐变描边，可以去掉 step
                float mask = step(0.1, outlineStrength); 
                
                fixed4 outlineColor = _OutlineColor * _OutlineIntensity * mask;
                outlineColor.a = saturate(outlineStrength * _OutlineColor.a);
                
                return outlineColor;
            }
            ENDCG
        }
        
        // ----------------------------------------------------------------
        // 3. 前向渲染 Base 通道 (ForwardBase)
        // ----------------------------------------------------------------
        Pass
        {
            Tags { "LightMode"="ForwardBase" "Queue" = "Geometry" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase fullforwardshadows
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Mask;
            float4 _Mask_ST;
            sampler2D _SpecularTex;
            float4 _SpecularTex_ST;
            float _SpecularLightScale;
            float4 _DiffuseColor;
            float _DiffuseLightScale;
            float _Steps;
            float _ToonEffect;
            
            float4 _Emission;
            float _EmissionScale;
            
            // 【修复 4】补充缺失的 Rim 变量声明
            float4 _RimColor;
            float _RimPower;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float2 specuv : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPosition : TEXCOORD3;
                float3 vertexLight : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _Mask);
                o.specuv = TRANSFORM_TEX(v.texcoord, _SpecularTex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                #ifdef LIGHTMAP_OFF
                o.vertexLight = ShadeSH9(float4(v.normal, 1));
                float3 vertexLight = Shade4PointLights(
                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    unity_4LightAtten0, o.worldPosition, o.worldNormal);
                o.vertexLight += vertexLight;
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                float4 albedo = tex2D(_MainTex, i.uv.xy);
                float3 mask = tex2D(_Mask, i.uv.zw);
                // float4 spec = tex2D(_SpecularTex, i.specuv); // 未使用，可注释
                
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPosition.xyz);
                
                float diffuseLight = saturate(dot(worldLightDir, i.worldNormal) * _DiffuseLightScale);
                
                // 自发光
                float3 emission = albedo.rgb * _Emission.rgb * _EmissionScale;
                
                // 卡通色阶
                float toon = floor(diffuseLight * _Steps) / _Steps;
                diffuseLight = lerp(diffuseLight, toon, _ToonEffect);
                
                float3 diffuse = _LightColor0.rgb * albedo.rgb * (1.0 - mask.g) * diffuseLight * _DiffuseColor.rgb;

                // 边缘光 (Rim)
                float rim = 1.0 - saturate(dot(i.worldNormal, worldViewDir));
                float3 rimColor = _RimColor.rgb * pow(rim, _RimPower);

                fixed shadow = SHADOW_ATTENUATION(i);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPosition);

                // 最终颜色组合
                float3 finalColor = ambient + rimColor + emission + (diffuse * shadow * atten);
                
                return float4(finalColor, 1.0);
            }
            ENDCG
        }

        // ----------------------------------------------------------------
        // 4. 前向渲染 Add 通道 (ForwardAdd)
        // ----------------------------------------------------------------
        Pass
        {
            Tags {"LightMode"="ForwardAdd" "Queue" = "Geometry"}
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Mask;
            float4 _Mask_ST;
            sampler2D _SpecularTex;
            float4 _SpecularTex_ST;
            float4 _DiffuseColor;
            float _DiffuseLightScale;
            float _Steps;
            float _ToonEffect;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
                float4 uv : TEXCOORD2;
                float2 specuv : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _Mask);
                o.specuv = TRANSFORM_TEX(v.texcoord, _SpecularTex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 albedo = tex2D(_MainTex, i.uv.xy);
                float3 mask = tex2D(_Mask, i.uv.zw);
                
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPosition));
                
                float diffuseLight = saturate(dot(worldLightDir, i.worldNormal) * _DiffuseLightScale);
                
                float toon = floor(diffuseLight * _Steps) / _Steps;
                diffuseLight = lerp(diffuseLight, toon, _ToonEffect);
                
                float3 diffuse = _LightColor0.rgb * albedo.rgb * (1.0 - mask.g) * diffuseLight * _DiffuseColor.rgb;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPosition);

                return float4(diffuse * atten, 1.0);
            }
            ENDCG
        }
    }
    
    // Fallback 建议改为一个简单的不透明 Shader，避免回退到 Standard 导致队列错误
    FallBack "Diffuse"
}