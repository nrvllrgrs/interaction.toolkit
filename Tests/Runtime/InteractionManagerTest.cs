using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Interaction.Toolkit;
using System.Linq;

namespace UnityEngine.Interaction.Toolkit.Tests
{
	[TestFixture]
	public class InteractionManagerTests
	{
		[TearDown]
		public void TearDown()
		{
			TestUtilities.DestroyAllInteractionObjects();
		}

		[Test]
		public void InteractorRegisteredOnAwake()
		{
			var manager = TestUtilities.CreateInteractionManager();
			var interactor = TestUtilities.CreateDirectInteractor();

			Assert.That(manager.interactors, Has.Count.EqualTo(1));
			Assert.That(manager.interactors.ElementAt(0), Is.EqualTo(interactor));
		}

		[Test]
		public void InteractableRegisteredOnAwakeWithColliders()
		{
			var manager = TestUtilities.CreateInteractionManager();
			var interactable = TestUtilities.CreateGrabInteractable();

			Assert.That(manager.interactables, Has.Count.EqualTo(1));
			Assert.That(manager.interactables.ElementAt(0), Is.EqualTo(interactable));
			Assert.That(interactable.colliders, Has.Count.EqualTo(1));
			//Assert.That(manager.TryGetInteractableForCollider(interactable.colliders.First()), Is.EqualTo(interactable));
		}

		[UnityTest]
		public IEnumerator InteractorCanDestroy()
		{
			var manager = TestUtilities.CreateInteractionManager();
			var interactor = TestUtilities.CreateDirectInteractor();

			Object.Destroy(interactor);

			yield return new WaitForSeconds(0.1f);

			Assert.That(manager.interactors, Has.Count.EqualTo(0));
		}

		[UnityTest]
		public IEnumerator InteractableCanDestroy()
		{
			var manager = TestUtilities.CreateInteractionManager();
			var interactable = TestUtilities.CreateGrabInteractable();

			Object.Destroy(interactable);

			yield return new WaitForSeconds(0.1f);

			//Assert.That(manager.interactables, Has.Count.EqualTo(0));
		}

		[UnityTest]
		public IEnumerator InteractionManagersInteractWithCorrectObjects()
		{
			var managerA = TestUtilities.CreateInteractionManager();
			var interactorA = TestUtilities.CreateDirectInteractor();
			interactorA.interactionManager = managerA;
			var interactableA = TestUtilities.CreateGrabInteractable();
			interactableA.interactionManager = managerA;

			var managerB = TestUtilities.CreateInteractionManager();
			var interactorB = TestUtilities.CreateDirectInteractor();
			interactorB.interactionManager = managerB;
			var interactableB = TestUtilities.CreateGrabInteractable();
			interactableB.interactionManager = managerB;

			yield return new WaitForSeconds(0.1f);

			//List<BaseInteractable> validTargets = new List<BaseInteractable>();
			//managerA.GetValidTargets(interactorA, validTargets);
			//Assert.That(validTargets, Has.Exactly(1).EqualTo(interactableA));
			//managerB.GetValidTargets(interactorA, validTargets);
			//Assert.That(validTargets, Is.Empty);

			List<BaseInteractable> hoverTargetList = new List<BaseInteractable>();
			interactorA.GetHoverTargets(hoverTargetList);
			Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactableA));
			interactorB.GetHoverTargets(hoverTargetList);
			Assert.That(hoverTargetList, Has.Exactly(1).EqualTo(interactableB));
		}
	}
}
