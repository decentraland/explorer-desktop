Shader "DCL/DepthToColorMap"
{
    Properties
    {
        _Strength ("Strength", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _Strength;

            v2f vert (const appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (const v2f i) : SV_Target
            {
                return pow(i.vertex.z / i.vertex.w, _Strength);
            }
            ENDCG
        }
    }
}
