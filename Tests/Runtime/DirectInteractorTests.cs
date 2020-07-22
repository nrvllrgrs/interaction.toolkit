using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Interaction.Toolkit;

namespace UnityEngine.Interaction.Toolkit.Tests
{
    [TestFixture]
    public class DirectInteractorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllInteractionObjects();
        }

        [UnityTest]
        public IEnumerator DirectInteractorCanHoverInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            var directInteractor = TestUtilities.CreateDirectInteractor();

            yield return new WaitForSeconds(0.1f);

            List<BaseInteractable> validTargets = new List<BaseInteractable>();
            //manager.GetValidTargets(directInteractor, validTargets);
            Assert.That(validTargets, Has.Exactly(1).EqualTo(interactable));

            List<BaseInteractable> hoverTargetList = new List<BaseInteractable>();
            directInteractor.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactable));
        }

        [UnityTest]
        public IEnumerator DirectInteractorCanSelectInteractable()
        {
            TestUtilities.CreateInteractionManager();
            var interactable = TestUtilities.CreateGrabInteractable();
            var directInteractor = TestUtilities.CreateDirectInteractor();

            yield return new WaitForSeconds(0.1f);

            Assert.That(directInteractor.selectTarget, Is.EqualTo(interactable));
        }
    }
}
