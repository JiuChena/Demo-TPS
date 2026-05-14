Shader "Custom/BRP_ViewDirectionOutline"
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
        _OutlineIntensity ("描边强度", Range(0.0, 5.0)) = 1.0
        _Steps("色阶阶数", Range(1,500)) = 3
        _ToonEffect("卡通渲染效果", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"} // 设置渲染队列，Geometry表示在不透明物体之后，透明物体之前渲染
        
        // --- Pass 1: 渲染描边 ---
        Pass
        {
            Name "OutlinePass"
            
            // 设置渲染状态
            Cull Front // 只渲染模型的背面（Front-facing polygons），制造描边效果
            ZWrite On // 写入深度缓冲，防止描边被后面的物体覆盖
            Blend SrcAlpha OneMinusSrcAlpha // 开启 Alpha 混合（如果描边需要透明度）

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // 包含 Unity 的内置函数库

            struct appdata
            {
                float4 vertex : POSITION; // 顶点位置
                float3 normal : NORMAL;   // 顶点法线
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // 裁剪空间坐标
                float3 viewDir : TEXCOORD0; // 世界空间下的视图方向
                float3 worldNormal : TEXCOORD1; // 世界空间下的法线
            };

            // Properties
            float4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                
                // 计算世界空间下的顶点位置和法线
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                // 计算世界空间下的视图方向 (从顶点指向摄像机)
                float3 viewDir = _WorldSpaceCameraPos - worldPos;
                
                // 将顶点沿法线方向外扩
                float3 outlineOffset = worldNormal * (_OutlineWidth / 1000);
                float4 outlineVertex = v.vertex + float4(outlineOffset, 0.0); // 外扩顶点
                
                o.pos = UnityObjectToClipPos(outlineVertex); // 转换到裁剪空间
                o.viewDir = viewDir;
                o.worldNormal = worldNormal;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 归一化视图方向和法线
                float3 viewDirNorm = normalize(i.viewDir);
                float3 worldNormalNorm = normalize(i.worldNormal);

                // 计算法线与视图方向的点积 (余弦值)
                float dotProduct = dot(worldNormalNorm, viewDirNorm);
                
                // 取绝对值，确保正面和背面的边缘都被检测到
                float edgeFactor = abs(dotProduct);
                
                // 使用 1 - edgeFactor 来反转，使边缘处接近 1，平面处接近 0
                float outlineStrength = 1.0 - edgeFactor;

                // 基于描边强度混合颜色
                // 你也可以直接使用 step 函数来创建更锐利的边缘
                // float outlineMask = step(_OutlineThreshold, outlineStrength); 
                // fixed4 finalColor = lerp(fixed4(0,0,0,0), _OutlineColor, outlineMask);

                // 计算最终描边颜色 (强度 * 颜色)
                fixed4 outlineColor = _OutlineColor * _OutlineIntensity * outlineStrength;
                
                // 为了更好的视觉效果，可以设置描边的 alpha，使其有渐变效果或半透明
                outlineColor.a = saturate(outlineStrength); // 使 alpha 也随强度变化
                
                return outlineColor;
            }
            ENDCG
        }

        // --- Pass 2: 渲染主体 ---
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fwdbase fullforwardshadows
            
            #include <Lighting.cginc>
            #include <UnityCG.cginc>
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

                fixed shadow = SHADOW_ATTENUATION(i);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPosition);

                return float4(ambient + emission + (diffuse) * atten, 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse" // 如果 SubShader 不支持，则使用 Diffuse 作为后备
}