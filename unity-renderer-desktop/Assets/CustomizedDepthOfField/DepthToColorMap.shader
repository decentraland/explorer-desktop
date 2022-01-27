Shader "DCL/DepthToColorMap"
{
    Properties
    {
        _Strength ("Strength", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
//        Offset -1000,-1000

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
                // float depth : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _Strength;

            v2f vert (const appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.depth = o.vertex.z / o.vertex.w;
                return o;
            }

            fixed4 frag (const v2f i) : SV_Target
            {
                return pow(i.vertex.z / i.vertex.w, _Strength);
                // return i.depth;
            }
            ENDCG
        }
    }
}
