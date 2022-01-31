using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomizedDepthOfField
{
    public class GaussianDepthOfFieldFixedRenderPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> shaderTags;
        private readonly Material overrideMaterial;
        private readonly int overrideMaterialPassIndex;
        private readonly bool blurSkybox;
        private readonly RenderTargetIdentifier destinationTextureIdentifier;
        private RenderTargetHandle destinationTextureHandle;
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;
        private RenderStateBlock stateBlock;

        public GaussianDepthOfFieldFixedRenderPass(RenderPassEvent renderPassEvent,
            RenderQueueRange renderQueueRange,
            LayerMask layerMask,
            RenderTargetHandle destinationTextureHandle,
            Material overrideMaterial,
            int overrideMaterialPassIndex,
            bool blurSkybox,
            IEnumerable<string> shaderTags)
        {
            profilingSampler = new ProfilingSampler(nameof(GaussianDepthOfFieldFixedRenderPass));
            filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            base.renderPassEvent = renderPassEvent;
            this.destinationTextureHandle = destinationTextureHandle;
            this.overrideMaterial = overrideMaterial;
            this.overrideMaterialPassIndex = overrideMaterialPassIndex;
            this.blurSkybox = blurSkybox;
            stateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            this.shaderTags = shaderTags.Select(tag => new ShaderTagId(tag)).ToList();
            destinationTextureIdentifier = new RenderTargetIdentifier(destinationTextureHandle.Identifier(), 0);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(destinationTextureHandle.id, descriptor, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                cmd.SetRenderTarget(destinationTextureIdentifier);
                cmd.ClearRenderTarget(true, true, blurSkybox ? Color.black : Color.white);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = filteringSettings.renderQueueRange == RenderQueueRange.all
                                || filteringSettings.renderQueueRange == RenderQueueRange.transparent
                    ? SortingCriteria.CommonTransparent
                    : renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = overrideMaterial;
                drawSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings, ref stateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(destinationTextureHandle.id);
        }
    }
}