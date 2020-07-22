using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Interaction.Toolkit;
using System;

namespace UnityEngine.Interaction.Toolkit.Tests
{
	public static class TestUtilities
	{
		internal static void DestroyAllInteractionObjects()
		{
			//foreach (var gameObject in Object.FindObjectsOfType<InteractionManager>())
			//{
			//	if (gameObject != null)
			//		Object.DestroyImmediate(gameObject.transform.root.gameObject);
			//}
			foreach (var gameObject in Object.FindObjectsOfType<BaseInteractable>())
			{
				if (gameObject != null)
					Object.DestroyImmediate(gameObject.transform.root.gameObject);
			}
			foreach (var gameObject in Object.FindObjectsOfType<BaseInteractor>())
			{
				if (gameObject != null)
					Object.DestroyImmediate(gameObject.transform.root.gameObject);
			}
		}
		internal static void CreateGOSphereCollider(GameObject go, bool isTrigger = true)
		{
			SphereCollider collider = go.AddComponent<SphereCollider>();
			collider.radius = 1.0f;
			collider.isTrigger = isTrigger;
		}

		internal static InteractionManager CreateInteractionManager()
		{
			GameObject managerGO = new GameObject();
			//InteractionManager manager = managerGO.AddComponent<InteractionManager>();
			//return manager;
			return null;
		}

		internal static TriggerInteractor CreateDirectInteractor()
		{
			GameObject interactorGO = new GameObject();
			CreateGOSphereCollider(interactorGO);
			TriggerInteractor interactor = interactorGO.AddComponent<TriggerInteractor>();
			return interactor;
		}

		internal static RayInteractor CreateRayInteractor()
		{
			GameObject interactorGO = new GameObject();
			interactorGO.name = "Ray Interactor";
			RayInteractor interactor = interactorGO.AddComponent<RayInteractor>();
			return interactor;
		}

		internal static GrabInteractable CreateGrabInteractable()
		{
			GameObject interactableGO = new GameObject();
			CreateGOSphereCollider(interactableGO, false);
			GrabInteractable interactable = interactableGO.AddComponent<GrabInteractable>();
			var rididBody = interactableGO.GetComponent<Rigidbody>();
			rididBody.useGravity = false;
			rididBody.isKinematic = true;
			return interactable;
		}    
	}
}
