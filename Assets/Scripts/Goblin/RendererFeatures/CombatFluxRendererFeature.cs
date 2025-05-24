using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Goblin.RendererFeatures
{
    public class CombatFluxRendererFeature : ScriptableRendererFeature
    {
        private CombatFluxPass pass { get; set; }
        
        public override void Create()
        {
            pass = new CombatFluxPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
        
        private class CombatFluxPass : ScriptableRenderPass
        {
            private CommandBuffer cmd { get; set; }
            private Material material { get; set; }

            public CombatFluxPass()
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
                material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                cmd = CommandBufferPool.Get("CombatFluxPass");
                cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}