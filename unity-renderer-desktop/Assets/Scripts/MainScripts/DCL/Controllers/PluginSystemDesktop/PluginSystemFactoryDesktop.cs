using DCL.Emotes;
using DCL.EmotesWheel;
using DCL.EquippedEmotes;
using DCL.ExperiencesViewer;
using DCL.Tutorial;
using DCL.Skybox;
using DCL.LogReport;

namespace DCL
{
    public static class PluginSystemFactoryDesktop
    {
        public static PluginSystem Create()
        {
            var pluginSystem = new PluginSystem();

            pluginSystem.Register(() => new DebugPluginFeature());
            pluginSystem.Register(() => new ShortcutsFeature());
            pluginSystem.Register(() => new ExploreV2FeatureDesktop());
            pluginSystem.Register(() => new DebugShapesBoundingBoxDisplayer());
            pluginSystem.Register(() => new TransactionFeature());
            pluginSystem.Register(() => new PreviewMenuPlugin());
            pluginSystem.Register(() => new SkyboxController());
            pluginSystem.Register(() => new GotoPanelPlugin());
            pluginSystem.Register(() => new ExperiencesViewerFeature());
            pluginSystem.Register(() => new EmoteAnimationsPlugin());
            pluginSystem.Register(() => new EquippedEmotesInitializerPlugin());
            pluginSystem.Register(() => new EmotesWheelUIPlugin());
            pluginSystem.Register(() => new LogReportPlugin());
            pluginSystem.RegisterWithFlag(() => new BuilderInWorldPlugin(), "builder_in_world");
            pluginSystem.RegisterWithFlag(() => new TutorialController(), "tutorial");
            pluginSystem.RegisterWithFlag(() => new PlacesAndEventsFeature(), "explorev2");
            pluginSystem.RegisterWithFlag(() => new SkyboxController(), "procedural_skybox");


            pluginSystem.SetFeatureFlagsData(DataStore.i.featureFlags.flags);

            return pluginSystem;
        }
    }
}
