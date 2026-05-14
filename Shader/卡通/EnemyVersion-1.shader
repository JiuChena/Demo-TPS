Shader "Unlit/EnemyVersion-1"
{
    Properties
    {
        _OutlineColor("描边颜色", Color) = (0,0,0,1)
        _OutlineWidth("描边宽度", Range(0, 5)) = 2
    }
    
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                if (v.normal.z < 0) v.normal.z = -v.normal.z;
                v.vertex.xyz += v.normal * _OutlineWidth / 1000;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(_OutlineColor);
            }
            
            ENDCG
        }
    }
}
