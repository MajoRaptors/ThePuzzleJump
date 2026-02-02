Shader "Custom/WeakenedCell"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.85, 0.7, 0.3, 1)
        _CrackTex ("Crack Texture", 2D) = "white" {}
        _CrackAmount ("Crack Amount", Range(0,1)) = 0
        _NormalStrength ("Normal Strength", Range(0,1)) = 0.6
        _SmoothnessMin ("Smoothness Min", Range(0,1)) = 0.1
        _SmoothnessMax ("Smoothness Max", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _CrackTex;

        struct Input
        {
            float2 uv_CrackTex;
        };

        half _CrackAmount;
        half _NormalStrength;
        half _SmoothnessMin;
        half _SmoothnessMax;
        fixed4 _BaseColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Sample crack texture (black = no crack, white = crack)
            float crack = tex2D(_CrackTex, IN.uv_CrackTex).r;

            // Mask controlled by CrackAmount
            float crackMask = smoothstep(_CrackAmount, _CrackAmount + 0.2, crack);

            // Darken cracks
            fixed3 albedo = lerp(
                _BaseColor.rgb,
                _BaseColor.rgb * 0.6,
                crackMask
            );

            o.Albedo = albedo;

            // Fake normal from cracks
            float3 normalFromCrack = UnpackNormal(
                tex2D(_CrackTex, IN.uv_CrackTex)
            );

            o.Normal = lerp(
                float3(0,0,1),
                normalFromCrack,
                crackMask * _NormalStrength
            );

            // Surface gets rougher when cracked
            o.Smoothness = lerp(
                _SmoothnessMax,
                _SmoothnessMin,
                _CrackAmount
            );

            o.Metallic = 0;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Standard"
}
