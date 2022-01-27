using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomizedDepthOfField
{
    [ExcludeFromPreset]
    public class CustomizedDepthOfFieldRenderFeature : ScriptableRendererFeature
    {
        private CustomizedDepthOfFieldRenderPass renderPass;
        public Settings settings = new Settings();

        public override void Create()
        {
            var filter = settings.filterSettings;

            // Render Objects pass doesn't support events before rendering prepasses.
            // The camera is not setup before this point and all rendering is monoscopic.
            // Events before BeforeRenderingPrepasses should be used for input texture passes (shadow map, LUT, etc) that doesn't depend on the camera.
            // These events are filtering in the UI, but we still should prevent users from changing it from code or
            // by changing the serialized data.
            if (settings.@event < RenderPassEvent.BeforeRenderingPrepasses)
                settings.@event = RenderPassEvent.BeforeRenderingPrepasses;

            var destinationTexture = new RenderTargetHandle();
            destinationTexture.Init("_CustomizedDepthOfField");
            renderPass = new CustomizedDepthOfFieldRenderPass(settings.@event,
                settings.filterSettings.RenderQueueRange, filter.LayerMask,
                destinationTexture,
                settings.overrideMaterial, settings.overrideMaterialPassIndex,
                settings.blurSkybox,
                settings.filterSettings.Passes);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(renderPass);
        }

        [Serializable]
        public class Settings
        {
            public RenderPassEvent @event = RenderPassEvent.AfterRenderingOpaques;
            public FilterSettings filterSettings = new FilterSettings();
            public CompareFunction depthCompareFunction = CompareFunction.Less;
            public Material overrideMaterial;
            public int overrideMaterialPassIndex;
            public bool blurSkybox;
        }

        public enum RenderQueueType
        {
            Opaque,
            Transparent,
            All
        }

        [Serializable]
        public class FilterSettings
        {
            public RenderQueueType RenderQueueType;
            public LayerMask LayerMask;

            public string[] Passes =
            {
                "SRPDefaultUnlit",
                "UniversalForward",
                "UniversalForwardOnly",
                "LightweightForward"
            };

            public FilterSettings()
            {
                RenderQueueType = RenderQueueType.Opaque;
                LayerMask = 0;
            }

            public RenderQueueRange RenderQueueRange
            {
                get
                {
                    return RenderQueueType switch
                    {
                        RenderQueueType.All => RenderQueueRange.all,
                        RenderQueueType.Opaque => RenderQueueRange.opaque,
                        RenderQueueType.Transparent => RenderQueueRange.transparent,
                        _ => RenderQueueRange.all
                    };
                }
            }
        }
    }
}