using NUnit.Framework;
using System.Collections;
using DCL;
using DCL.Controllers;
using UnityEngine.TestTools;

namespace Tests
{
    public class MainDesktopShould : IntegrationTestSuite
    {
        private ParcelScene scene;

        protected override void InitializeServices(ServiceLocator serviceLocator)
        {
            serviceLocator.Register<ISceneController>(() => new SceneController());
            serviceLocator.Register<IWorldState>(() => new WorldState());
            serviceLocator.Register<IRuntimeComponentFactory>(() => new RuntimeComponentFactory());
        }

        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = Environment.i.world.sceneController.CreateTestScene() as ParcelScene;
        }

        [Ignore("Will fix this later")] // i promise
        [UnityTest]
        public IEnumerator BeTrue()
        {
            Assert.IsTrue(true);
            yield return null;
        }
    }
}