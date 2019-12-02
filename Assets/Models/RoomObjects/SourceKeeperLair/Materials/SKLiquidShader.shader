Shader "Custom/SKLiquidShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert
        #pragma target 3.0

        float getOffset(float normalizedDistanceToCenter)
        {
            const float peakHeight = 0.08;
            float transformMod = pow(1.0 - normalizedDistanceToCenter, 1);
            float offset = cos(_Time.z + pow(transformMod, 2) * 6.28) * 0.5 + 0.5;
            return offset * peakHeight * transformMod;
        }

        void vert (inout appdata_full v)
        {
            float distanceToCenter = sqrt(v.vertex.x*v.vertex.x + v.vertex.z*v.vertex.z);
            float normalizedDistanceToCenter = distanceToCenter / 0.605;

            const float neighbourDistance = 0.1;
            float offset = getOffset(normalizedDistanceToCenter);
            float prevOffset = getOffset(normalizedDistanceToCenter - neighbourDistance);
            float nextOffset = getOffset(normalizedDistanceToCenter + neighbourDistance);

            v.vertex.y += offset;
            if (distanceToCenter == 0.0)
            {
                v.normal = float3(0.0, 1.0, 0.0);
            }
            else
            {
              float2 normal2D = normalize(normalize(float2(offset - prevOffset, neighbourDistance)) + normalize(float2(nextOffset - offset, neighbourDistance)));
              float2 XYdistribution = normalize(v.vertex.xz);
              v.normal = normalize(float3(XYdistribution.x * normal2D.x, normal2D.y, XYdistribution.y * normal2D.x));
            }
        }

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
