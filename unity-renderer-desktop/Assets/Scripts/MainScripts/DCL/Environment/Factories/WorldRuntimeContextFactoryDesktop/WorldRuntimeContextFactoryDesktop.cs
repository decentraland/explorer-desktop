using DCL;
using DCL.Controllers;
using MainScripts.DCL.WorldRuntime.RuntimeComponentFactoryDesktop;

namespace MainScripts.DCL.Environment.Factories.WorldRuntimeContextFactoryDesktop
{
    public static class WorldRuntimeContextFactoryDesktop
    {
        public static WorldRuntimeContext CreateDefault()
        {
            return CreateDefault(null);
        }
        public static WorldRuntimeContext CreateDefault(IPoolableComponentFactory poolableComponentFactory)
        {
            return new WorldRuntimeContext(
                state: new WorldState(),
                sceneController: new SceneController(),
                pointerEventsController: new PointerEventsController(),
                sceneBoundsChecker: new SceneBoundsChecker(),
                blockersController: new WorldBlockersController(),
                componentFactory: new RuntimeComponentFactoryDesktop(poolableComponentFactory));
        }
    }
}