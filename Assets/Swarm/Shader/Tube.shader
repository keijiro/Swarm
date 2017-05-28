Shader "Swarm/Tube"
{
    Properties
    {
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
            half param : COLOR;
        };

        half _Smoothness;
        half _Metallic;

        half3 _GradientA;
        half3 _GradientB;
        half3 _GradientC;
        half3 _GradientD;

        float4x4 _LocalToWorld;
        float4x4 _WorldToLocal;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

        StructuredBuffer<float4> _PositionBuffer;
        uint _InstanceCount;
        uint _HistoryLength;
        uint _IndexOffset;

        #endif

        half3 CosineGradient(half param)
        {
            half3 c = _GradientB * cos(_GradientC * param + _GradientD);
            return GammaToLinearSpace(saturate(c + _GradientA));
        }

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

            const float radius = 0.01;

            float phi = v.vertex.x;
            float seg = v.vertex.z;

            uint id = unity_InstanceID;
            uint idx = clamp(seg, 1, _HistoryLength - 2);
            uint idx0 = (idx - 1 + _IndexOffset + _HistoryLength) % _HistoryLength;
            uint idx1 = (idx     + _IndexOffset) % _HistoryLength;
            uint idx2 = (idx + 1 + _IndexOffset) % _HistoryLength;
            uint idx3 = (idx + 2 + _IndexOffset) % _HistoryLength;

            float3 p0 = _PositionBuffer[id + idx0 * _InstanceCount].xyz;
            float3 p1 = _PositionBuffer[id + idx1 * _InstanceCount].xyz;
            float3 p2 = _PositionBuffer[id + idx2 * _InstanceCount].xyz;
            float3 p3 = _PositionBuffer[id + idx3 * _InstanceCount].xyz;

            float3 vt0 = normalize(p2 - p0);
            float3 vt1 = normalize(p3 - p1);
            float3 vn = normalize(vt1 - vt0);
            float3 vb = cross(vt1, vn);
            vn = cross(vb, vt1);

            float2 xy = float2(cos(phi), sin(phi));

            float param = seg / _HistoryLength;
            param += (float)unity_InstanceID / _InstanceCount; 

            v.vertex.xyz = p1 + (vn * xy.x + vb * xy.y) * radius;
            v.normal.xyz = vn * xy.x + vb * xy.y;
            v.color = param;

            #endif
        }

        void setup()
        {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            unity_ObjectToWorld = _LocalToWorld;
            unity_WorldToObject = _WorldToLocal;
            #endif
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = CosineGradient(IN.param);
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
