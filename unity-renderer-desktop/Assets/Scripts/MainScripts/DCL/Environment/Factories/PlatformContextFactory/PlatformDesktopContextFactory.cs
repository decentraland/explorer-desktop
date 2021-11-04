using DCL.Rendering;
using UnityEngine;

namespace DCL
{
    public static class PlatformDesktopContextFactory
    {
        public static PlatformContext CreateDefault()
        {
            if (InitialSceneReferences.i != null)
                return CreateDefault(InitialSceneReferences.i.bridgeGameObject);

            return CreateDefault(null);
        }

        public static PlatformContext CreateDefault(GameObject bridgesGameObject)
        {
            return new PlatformContext(
                memoryManager: new MemoryManager(8192L * 1024L * 1024L, 30.0f), // 8gb and clean each 30 seconds
                cullingController: CullingController.Create(),
                clipboard: Clipboard.Create(),
                physicsSyncController: new PhysicsSyncController(),
                parcelScenesCleaner: new ParcelScenesCleaner(),
                webRequest: WebRequestController.Create(),
                serviceProviders: new ServiceProviders(),
                idleChecker: new IdleChecker(),
                avatarsLODController: new AvatarsLODController(),
                featureFlagController: new FeatureFlagController(bridgesGameObject));
        }
    }
}