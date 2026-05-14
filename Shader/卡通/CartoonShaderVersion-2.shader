Shader "CartoonShader/Character Shader/Version2"
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
        _OutlineColor("描边颜色", Color) = (0,0,0,1)
        _OutlineWidth("描边宽度(mm)", Range(0, 5)) = 0.005
        _Steps("色阶阶数", Range(1,500)) = 3
        _ToonEffect("卡通渲染效果", Range(0,1)) = 0.5
        _RimColor("边缘光颜色", Color) = (1,1,1,1)
        _RimPower("边缘光强度", Range(0.01, 1)) = 0.03
        _XRayColor("遮挡透视描边颜色", Color) = (1,1,1,1)
        _XRayEnhancement("透视描边强度", Range(1, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+100"}
        LOD 100
        
        //透视通道
        Pass
        {
            Tags { "ForceNoShadowcasting"="True" }
            
            Cull Front
            ZWrite Off
            ZTest Greater
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <UnityCG.cginc>

            float3 _XRayColor;
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

            float4 frag(v2f i) : SV_Target
            {
                return float4(_XRayColor * _XRayEnhancement, 1);
            }
            
            ENDCG
        }

        //描边通道
        Pass
        {
            Name "Outline"
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <UnityCG.cginc>

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata_base v)
            {
                v2f o;
                float4 pos = mul (UNITY_MATRIX_MV, v.vertex) ; 
                float3 normal= mul((float3x3)UNITY_MATRIX_IT_MV, v .normal); 
                normal. z = -0.5; 
                pos = pos + float4(normalize(normal) , 0) * (_OutlineWidth / 1000); 
                o.pos = mul(UNITY_MATRIX_P, pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(_OutlineColor);
            }
            
            ENDCG
        }
        
        //前向渲染Base通道
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fwdbase fullforwardshadows

            #include <UnityCG.cginc>
            #include <Lighting.cginc>
            #include <AutoLight.cginc>
            

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
            float3 _RimColor;
            float _RimPower;

            float4 _Emission;
            float _EmissionScale;

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
                
                float3 vertexLight = Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    unity_4LightAtten0, o.worldPosition, o.worldNormal);
                o.vertexLight += vertexLight;
                #endif

                TRANSFER_SHADOW(o);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                float4 albedo = tex2D(_MainTex, i.uv.xy);
                float3 mask = tex2D(_Mask, i.uv.zw);
                float4 spec = tex2D(_SpecularTex, i.specuv);
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPosition.xyz);
                float3 halfDir = normalize(worldLightDir + worldViewDir);
                float diffuseLight = saturate(dot(worldLightDir, i.worldNormal) * _DiffuseLightScale);
                float3 emission = albedo.rgb * _Emission * _EmissionScale;
                //漫反射颜色离散化
                float toon = floor(diffuseLight * _Steps) / _Steps;
                diffuseLight = lerp(diffuseLight, toon, _ToonEffect);
                float3 diffuse = _LightColor0.rgb * albedo.rgb * (1 - mask.g) * diffuseLight * _DiffuseColor.rgb;

                float rim = 1 - dot(i.worldNormal, worldViewDir);
                float3 rimColor = _RimColor * pow(rim, 1 / _RimPower);

                fixed shadow = SHADOW_ATTENUATION(i);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPosition);

                return float4(ambient + rimColor + emission + (diffuse) * atten, 1);
            }
            ENDCG
        }

        //前向渲染Add通道
        Pass
        {
            Tags {"LightMode"="ForwardAdd"}
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include <UnityCG.cginc>
            #include <Lighting.cginc>
            #include <AutoLight.cginc>

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

                TRANSFER_VERTEX_TO_FRAGMENT(o)

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 albedo = tex2D(_MainTex, i.uv.xy);
                float3 mask = tex2D(_Mask, i.uv.zw);
                float3 spec = tex2D(_SpecularTex, i.specuv);
                
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPosition));
                fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);
                
                float diffuseLight = saturate(dot(worldLightDir, i.worldNormal) * _DiffuseLightScale);
                // //漫反射颜色离散化
                float toon = floor(diffuseLight * _Steps) / _Steps;
                diffuseLight = lerp(diffuseLight, toon, _ToonEffect);
                float3 diffuse = _LightColor0.rgb * albedo.rgb * (1 - mask.g) * diffuseLight * _DiffuseColor.rgb;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPosition);

                return float4((diffuse) * atten, 1);
            }
            ENDCG
        }
        
    }
    Fallback "Diffuse"
}
