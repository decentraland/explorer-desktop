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

        protected override WorldRuntimeContext CreateRuntimeContext()
        {
            return DCL.Tests.WorldRuntimeContextFactory.CreateWithCustomMocks
            (
                sceneController: new SceneController(),
                state: new WorldState(),
                componentFactory: new RuntimeComponentFactory()
            );
        }

        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = Environment.i.world.sceneController.CreateTestScene() as ParcelScene;
        }

        [UnityTest]
        public IEnumerator BeTrue()
        {
            Assert.IsTrue(true);
            yield return null;
        }
    }
}