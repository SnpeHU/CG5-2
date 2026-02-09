using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
[CreateAssetMenu(fileName = "PostEffectRenderPass", menuName = "Scriptable Objects/PostEffectRenderPass")]
public class PostEffectRenderPass : ScriptableRenderPass
{
    private Material postEffectMaterial_ = null;
    private Material passThroughMaterial_ = null;
    public PostEffectRenderPass(Material blurMat, Material passThroughMat)
    {
        postEffectMaterial_ = blurMat;
        passThroughMaterial_ = passThroughMat;
        //renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if(postEffectMaterial_ == null || passThroughMaterial_ == null)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }
        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        if(resources.isActiveTargetBackBuffer)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        // UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        // if(resourceData.isActiveTargetBackBuffer)
        // {
        //     return;
        // }

        TextureHandle cameraTexture = resources.activeColorTexture;

        TextureDesc tempDesc = renderGraph.GetTextureDesc(cameraTexture);
        tempDesc.name = "_OrigTempTexture";
        tempDesc.depthBufferBits = 0;

        TextureHandle tempTexture = renderGraph.CreateTexture(tempDesc);

        

        RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = 
            new RenderGraphUtils.BlitMaterialParameters(cameraTexture, tempTexture, postEffectMaterial_, 0);

            renderGraph.AddBlitPass(blitMaterialParameters, "BlitGreenPostEffect");

            //renderGraph.AddCopyPass(tempTexture, cameraTexture, "CopyBackToCameraTexture");
            renderGraph.AddCopyPass(tempTexture, cameraTexture, "CopyGreenPostEffect");

    }
     
}

   

