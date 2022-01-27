using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomizedDepthOfField
{
    public class CustomizedDepthOfFieldRenderPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> shaderTags;
        private RenderTargetHandle depthAttachmentHandle;
        private readonly Material overrideMaterial;
        private readonly int overrideMaterialPassIndex;
        private readonly bool blurSkybox;
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;
        private RenderStateBlock stateBlock;
        private RenderTargetIdentifier renderTargetIdentifier;

        public CustomizedDepthOfFieldRenderPass(RenderPassEvent renderPassEvent,
            RenderQueueRange renderQueueRange,
            LayerMask layerMask,
            RenderTargetHandle depthAttachmentHandle,
            Material overrideMaterial,
            int overrideMaterialPassIndex,
            CompareFunction settingsDepthCompareFunction,
            bool blurSkybox,
            IEnumerable<string> shaderTags)
        {
            profilingSampler = new ProfilingSampler(nameof(CustomizedDepthOfFieldRenderPass));
            filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            base.renderPassEvent = renderPassEvent;
            this.depthAttachmentHandle = depthAttachmentHandle;
            this.overrideMaterial = overrideMaterial;
            this.overrideMaterialPassIndex = overrideMaterialPassIndex;
            this.blurSkybox = blurSkybox;
            stateBlock = new RenderStateBlock(RenderStateMask.Depth)
            {
                depthState = new DepthState(true, settingsDepthCompareFunction)
            };
            this.shaderTags = shaderTags.Select(tag => new ShaderTagId(tag)).ToList();
            renderTargetIdentifier = new RenderTargetIdentifier(depthAttachmentHandle.Identifier(), 0);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(depthAttachmentHandle.id, descriptor, FilterMode.Bilinear);
            ConfigureTarget(renderTargetIdentifier);
            ConfigureClear(ClearFlag.Color, blurSkybox ? Color.black : Color.white);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = filteringSettings.renderQueueRange == RenderQueueRange.all
                                || filteringSettings.renderQueueRange == RenderQueueRange.transparent
                    ? SortingCriteria.CommonTransparent
                    : renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = overrideMaterial;
                drawSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings,
                    ref stateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(depthAttachmentHandle.id);
        }
    }
}