Shader "Cloner/Swarm"
{
	Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalScale("Normal Scale", Range(0, 1)) = 1
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metallic", Range(0, 1)) = 0
	}
	SubShader
    {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM

		#pragma surface surf Standard vertex:vert addshadow
        #pragma instancing_options procedural:setup

		struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        half _NormalScale;
		half _Smoothness;
		half _Metallic;

        half3 _GradientA;
        half3 _GradientB;
        half3 _GradientC;
        half3 _GradientD;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

        StructuredBuffer<float4> _PositionBuffer;
        uint _ArraySize;
        uint _InstanceCount;

        #endif

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

            const float radius = 0.01;

            float phi = v.vertex.x;
            int seg = (int)v.vertex.z;
            float cp = v.vertex.z / _ArraySize + (float)unity_InstanceID / _InstanceCount; 

            seg = clamp(seg, 2, _ArraySize - 1) - 2;
            seg = unity_InstanceID + seg * _InstanceCount;

            float3 p0 = _PositionBuffer[seg].xyz; seg += _InstanceCount;
            float3 p1 = _PositionBuffer[seg].xyz; seg += _InstanceCount;
            float3 p2 = _PositionBuffer[seg].xyz; seg += _InstanceCount;
            float3 p3 = _PositionBuffer[seg].xyz;

            float3 vt0 = normalize(p2 - p0);
            float3 vt1 = normalize(p3 - p1);
            float3 vn = normalize(vt1 - vt0);
            float3 vb = cross(vt1, vn);
            vn = cross(vb, vt1);

            float2 xy = float2(cos(phi), sin(phi));

            v.vertex.xyz = p1 + (vn * xy.x + vb * xy.y) * radius;
            v.normal.xyz = vn * xy.x + vb * xy.y;
            v.texcoord = cp;

        #endif
        }

        void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

        #endif
        }

		void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;
            half3 gradient = saturate(_GradientA + _GradientB * cos(_GradientC * uv.y + _GradientD));
			o.Albedo = tex2D(_MainTex, uv).rgb * GammaToLinearSpace(gradient);
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
            o.Normal = UnpackScaleNormal(tex2D(_NormalMap, uv), _NormalScale);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
