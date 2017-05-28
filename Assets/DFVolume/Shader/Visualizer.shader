// DFVolume - Distance field volume generator for Unity
// https://github.com/keijiro/DFVolume

Shader "Hidden/DFVolume/Visualizer"
{
	Properties
	{
		_MainTex("", 3D) = "white" {}
	}

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD;
    };

    sampler3D _MainTex;
    float _Depth;
    float _Mode;
    
    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = v.texcoord;
        return o;
    }
    
    fixed4 frag(v2f i) : SV_Target
    {
        fixed4 data = tex3D(_MainTex, float3(i.texcoord, _Depth));
        fixed dist = abs(0.5 - frac(data.a * 10)) * 2;
        fixed3 grad = data.rgb + 0.5;
        return fixed4(lerp(dist, grad, _Mode), 1);
    }

    ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
