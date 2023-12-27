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
        #include "Assets/01_Scripts/Effect/rand.hlsl"

        struct Attributes 
        {
            float4 positionOS	: POSITION;
            float3 normalOS : NORMAL;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings 
        {
            float4 positionCS 	: SV_POSITION;
            uint id : SV_InstanceID;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
            // UNITY_VERTEX_OUTPUT_STEREO
        };

        struct DashLine
        {
            float2 pos;
            float4 color;
        };

        StructuredBuffer<DashLine> _InstanceBuffer;

        Varyings vert (Attributes input, uint id : SV_InstanceID)
        {
            Varyings output;
            const float3 offset = float3(_InstanceBuffer[id].pos, 0);

            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_TRANSFER_INSTANCE_ID(input, output);
            // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz + offset);

            output.positionCS = TransformWorldToHClip(positionInputs.positionWS);
            output.id = id;
            return output;
        }

        float4 frag (Varyings input) : SV_Target
        {
            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float4 color = _InstanceBuffer[input.id].color;
            clip(color);
            return color;
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
