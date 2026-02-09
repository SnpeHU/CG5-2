using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "PostEffectRenderFeature", menuName = "Scriptable Objects/PostEffectRenderFeature")]
public class PostEffectRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material postEffectMaterial;
	[SerializeField]
	private Material passThroughMaterial;
    private PostEffectRenderPass renderPass;
	public override void Create()
	{
		//renderPass = new PostEffectRenderPass(postEffectMaterial, passThroughMaterial);
		renderPass = new PostEffectRenderPass(postEffectMaterial, passThroughMaterial);
        renderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (postEffectMaterial != null && passThroughMaterial != null)
        {
            renderer.EnqueuePass(renderPass);
        }
	}
}
