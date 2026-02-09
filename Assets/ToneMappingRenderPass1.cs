using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
[CreateAssetMenu(fileName = "ToneMappingRenderPass", menuName = "Scriptable Objects/ToneMappingRenderPass")]
public class ToneMappingRenderPass : ScriptableRenderPass
{
    private Material material_ = null;
    public ToneMappingRenderPass(Material mat)
    {
        material_ = mat;
        //renderPassEvent = RenderPassEvent.AfterRendering;
    }


    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if(material_ == null)
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

       TextureHandle cameraTexture = resources.activeColorTexture;

       TextureDesc tempDesc = renderGraph.GetTextureDesc(cameraTexture);

        tempDesc.name = "_ToneMapping";
        tempDesc.depthBufferBits = 0;

        TextureHandle tempTexture = renderGraph.CreateTexture(tempDesc);

        RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = 
            new RenderGraphUtils.BlitMaterialParameters(cameraTexture, tempTexture, material_, 0);

            renderGraph.AddBlitPass(blitMaterialParameters, "BlitToneMapping");

            renderGraph.AddCopyPass(tempTexture, cameraTexture, "CopyToneMapping");


    }
     
}

   

