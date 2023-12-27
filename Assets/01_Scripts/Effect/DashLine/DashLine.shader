Shader "Hidden/DashLine"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct Attributes 
        {
            float4 positionOS	: POSITION;
            float3 normalOS : NORMAL;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings 
        {
            float4 positionCS 	: SV_POSITION;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
            // UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert (Attributes input)
        {
            Varyings output;

            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_TRANSFER_INSTANCE_ID(input, output);
            // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);

            output.positionCS = positionInputs.positionCS;
            return output;
        }

        float4 frag (Varyings input) : SV_Target
        {
            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            return 0.5;
        }
        ENDHLSL

        Pass
        {
            Tags{"LightMode"="SRPDefaultUnlit"}
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            
            ENDHLSL
        }

        Pass
        {
            Tags{"LightMode"="ShadowCaster"}
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma  multi_compile_shadowcaster
            
            ENDHLSL
        }

        Pass
        {
            Tags 
            {
                "LightMode"="DepthOnly"
            }
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
         
        Pass
        {
            Tags 
            {
                "LightMode"="DepthNormalsOnly"
            }
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
    }
}
