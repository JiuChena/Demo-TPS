Shader "Unlit/Decal"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("贴花纹理 (RGB) 透明度 (A)", 2D) = "white" {}
        _Color ("颜色乘数", Color) = (1,1,1,1)
        
        [NoScaleOffset] _NormalTex ("法线贴图 (可选)", 2D) = "bump" {}
        _NormalStrength ("法线强度", Range(0, 2)) = 1.0
        
        // 深度偏移值，防止闪烁
        _DepthBias ("深度偏移", Range(-0.1, 0.1)) = -0.02
    }

    SubShader
    {
        // 标签：设置为透明，不写入深度
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        LOD 100
        
        // 关闭光照（贴花通常是自发光或覆盖色）
        Lighting Off
        // 关闭深度写入，但保留深度测试（防止被墙挡住）
        ZWrite Off
        // 深度偏移，这是贴花不闪烁的关键
        Offset [_DepthBias], 0
        // 关闭剔除，确保投影器背面也能计算（虽然通常只渲染正面）
        Cull Off

        // 混合模式：源颜色 * 源Alpha + 目标颜色 * (1 - 源Alpha)
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "DecalBase"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            // --- 属性变量 ---
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            sampler2D _NormalTex;
            float _NormalStrength;

            // --- Projector 专用矩阵 ---
            // 这些变量由 Projector 组件自动赋值
            float4x4 unity_Projector; 
            float4x4 unity_ProjectorClip;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0; // xy = UV, zw = 深度/衰减
                float3 normalDir : TEXCOORD1;
            };

            // --- 顶点着色器 ---
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                // 计算世界空间法线，用于简单的法线混合
                o.normalDir = UnityObjectToWorldNormal(v.normal);

                // --- 核心投影逻辑 ---
                // 将顶点位置转换到投影器的空间
                float4 projectorPos = mul(unity_Projector, v.vertex);
                
                // 计算 UV：将投影坐标映射到 0~1 范围
                // projectorPos.xyz / projectorPos.w 是透视除法
                o.uv.xy = projectorPos.xy / projectorPos.w * 0.5 + 0.5;
                
                // 计算深度/衰减（可选，用于边缘柔化）
                // 这里简单使用 z 分量
                o.uv.z = projectorPos.z / projectorPos.w;

                return o;
            }

            // --- 片元着色器 ---
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 采样颜色
                fixed4 col = tex2D(_MainTex, i.uv.xy) * _Color;

                // 2. 简单的边缘衰减（可选，防止投影无限延伸）
                // 如果 UV 超出 0-1 范围，强制透明
                if (i.uv.x < 0 || i.uv.x > 1 || i.uv.y < 0 || i.uv.y > 1)
                {
                    discard;
                }

                // 3. 法线混合逻辑 (简化版)
                // 注意：在 BRP 中，直接修改 GBuffer 比较复杂。
                // 这里我们仅输出颜色。如果需要法线效果，通常需要配合延迟渲染管线
                // 或者使用更复杂的 Shader 写入 _CameraNormalsTexture（不推荐，性能开销大）。
                // 对于大多数 BRP 贴花，仅使用透明混合颜色即可。
                
                // 如果需要法线贴图的透明细节（如裂纹），可以直接在这里混合
                // fixed3 normalTangent = UnpackNormal(tex2D(_NormalTex, i.uv.xy));
                
                return col;
            }
            ENDCG
        }
    }
}