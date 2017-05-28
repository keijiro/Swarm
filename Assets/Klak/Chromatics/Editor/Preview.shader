// Preview shader for cosine gradient
Shader "Hidden/Klak/Chromatics/CosineGradient/Preview"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    half3 _CoeffsA;
    half3 _CoeffsB;
    half3 _CoeffsC;
    half3 _CoeffsD;

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    half4 frag (v2f i) : SV_Target
    {
        half t = i.uv.x;

        half3 rgb = saturate(_CoeffsA + _CoeffsB * cos(_CoeffsC * t + _CoeffsD));

        #if !defined(UNITY_COLORSPACE_GAMMA)
        rgb = GammaToLinearSpace(rgb);
        #endif

        return half4(rgb, 1);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
