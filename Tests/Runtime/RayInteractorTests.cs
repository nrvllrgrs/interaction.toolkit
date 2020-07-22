using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Interaction.Toolkit;

namespace UnityEngine.Interaction.Toolkit.Tests
{
    [TestFixture]
    public class RayInteractorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllInteractionObjects();
        }

        [UnityTest]
        public IEnumerator RayInteractorCanHoverInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            yield return new WaitForSeconds(0.1f);

            List<BaseInteractable> validTargets = new List<BaseInteractable>();
            //manager.GetValidTargets(interactor, validTargets);
            Assert.That(validTargets, Has.Exactly(1).EqualTo(interactable));

            List<BaseInteractable> hoverTargetList = new List<BaseInteractable>();
            interactor.GetHoverTargets(hoverTargetList);
            Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactable));
        }

        [UnityTest]
        public IEnumerator RayInteractorCanSelectInteractable()
        {
            var manager = TestUtilities.CreateInteractionManager();
            var interactor = TestUtilities.CreateRayInteractor();
            interactor.transform.position = Vector3.zero;
            interactor.transform.forward = Vector3.forward;
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.transform.position = interactor.transform.position + interactor.transform.forward * 5.0f;

            yield return new WaitForSeconds(0.1f);

            Assert.That(interactor.selectTarget, Is.EqualTo(interactable));
        }
    }
}
