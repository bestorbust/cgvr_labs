
Shader "Custom/RGBVertexShader"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "RGB Triangle"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                // Vertex shader: object space to clip space
                output.positionCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                // Pass RGB vertex colour to fragment shader
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Fragment shader: interpolated RGB colour
                return half4(input.color.rgb, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
