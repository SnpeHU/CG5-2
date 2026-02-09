using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "ToneMappingRenderFeature", menuName = "Scriptable Objects/ToneMappingRenderFeature")]
public class ToneMappingRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material ToneMappingMaterial;
    private ToneMappingRenderPass renderPass;
	public override void Create()
	{
		renderPass = new ToneMappingRenderPass(ToneMappingMaterial);
        renderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (ToneMappingMaterial != null)
        {
            renderer.EnqueuePass(renderPass);
        }
	}
}
