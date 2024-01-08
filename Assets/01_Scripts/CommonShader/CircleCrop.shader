Shader "Custom/CircleCrop"
{
    Properties
    {
        _Progress("Progress", Range(0, 1)) = 0
        _CircleRatio("CircleRatio", Float) = 1
        _CenterScreenPos("Center Screen Position", Vector) = (0, 0, 0, 0)
        
        [HideInInspector][Enum(UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenBlendMode)]_Fullscreen_BlendMode("Float", Float) = 1
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_SrcColorBlend("Float", Float) = 5
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_DstColorBlend("Float", Float) = 10
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_SrcAlphaBlend("Float", Float) = 1
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_DstAlphaBlend("Float", Float) = 10
        [HideInInspector][Enum(UnityEditor.ShaderGraph.BlendOp)]_Fullscreen_ColorBlendOperation("Float", Float) = 0
        [HideInInspector][Enum(UnityEditor.ShaderGraph.BlendOp)]_Fullscreen_AlphaBlendOperation("Float", Float) = 0
        [HideInInspector][ToggleUI]_Fullscreen_DepthWrite("Boolean", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            // RenderType: <None>
            // Queue: <None>
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalFullscreenSubTarget"
        }
        Pass
        {
            Name "DrawProcedural"
        
        // Render State
        Cull Off
        Blend [_Fullscreen_SrcColorBlend] [_Fullscreen_DstColorBlend], [_Fullscreen_SrcAlphaBlend] [_Fullscreen_DstAlphaBlend]
        BlendOp [_Fullscreen_ColorBlendOperation], [_Fullscreen_AlphaBlendOperation]
        ZTest Off
        ZWrite [_Fullscreen_DepthWrite]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DRAWPROCEDURAL
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        float _Progress;
        float _CircleRatio;
        float4 _CenterScreenPos;

        // https://gl-transitions.com/editor/CircleCrop
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;

            float2 screenUV = IN.uv0.xy;
            _CenterScreenPos.x /= _ScreenParams.x;
            _CenterScreenPos.y /= _ScreenParams.y;
            screenUV -= _CenterScreenPos;

            screenUV.y *= _ScreenParams.x / _ScreenParams.y; // 1980 / 1080 * 1
            
            float ratio = 3;
            float2 ratio2 = float2(1, 1 / ratio);
            float progress = _Progress / 2;
            
            float s = pow(2.0 * abs(progress - 0.55), 3.0);

            float dist = length((screenUV) * ratio2);
            
            surface.BaseColor = 0;
            surface.Alpha = step(s, dist);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
            output.uv0 = input.texCoord0;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenDrawProcedural.hlsl"
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenShaderGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}