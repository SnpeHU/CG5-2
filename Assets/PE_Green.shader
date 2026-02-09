Shader "CustomRenderTexture/Green"
{

     SubShader
     {
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            ZWrite Off
            Cull Off
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

           half4 Frag(Varyings input) : SV_Target
           {
               half4 output = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat,input.texcoord);

                output.r = 0.0;
                output.b = 0.0;
                return output;
           }
            ENDHLSL
        }
    }
}
