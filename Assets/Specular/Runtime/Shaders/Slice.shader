Shader "Specular/Slice"{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _SliceNormal("normal", Vector) = (0,0,0,0)
        _SliceCentre ("centre", Vector) = (0,0,0,0)
        _SliceOffsetDst("offset", Float) = 0
    }
    SubShader {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Geometry" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard addshadow
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // World space normal of slice, anything along this direction from centre will be invisible
        float3 _SliceNormal;
        // World space centre of slice
        float3 _SliceCentre;
        // Increasing makes more of the mesh visible, decreasing makes less of the mesh visible
        float _SliceOffsetDst;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 adjustedCentre = _SliceCentre + _SliceNormal * _SliceOffsetDst;
            float3 offsetToSliceCentre = adjustedCentre - IN.worldPos;
            clip (dot(offsetToSliceCentre, _SliceNormal));

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
