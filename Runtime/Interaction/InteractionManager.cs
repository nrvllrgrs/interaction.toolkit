using System.Collections.Generic;

namespace UnityEngine.Interaction.Toolkit
{
	/// <summary>
	/// The Interaction Manager acts as an intermediary between Interactors and Interactables in a scene.  
	/// It is possible to have multiple Interaction Managers, each with their own valid set of Interactors and Interactables.  
	/// Upon Awake both Interactors and Interactables register themselves with a valid Interaction Manager in the scene 
	/// (if a specific one has not already been assigned in the inspector).  Every scene must have at least one Interaction Mananger
	/// for Interactors and Interactables to be able to communicate.
	/// </summary>
	/// <remarks>
	/// Many of the methods on this class are designed to be internal such that they can be called by the abstract
	/// base classes of the Interaction system (but are not called directly).
	/// </remarks>
	[AddComponentMenu("Interaction/Interaction Manager")]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(InteractionUpdateOrder.k_InteractionManager)]

	public class InteractionManager : Singleton<InteractionManager>
	{        
		List<BaseInteractor> m_Interactors 	= new List<BaseInteractor>();
		List<BaseInteractable> m_Interactables = new List<BaseInteractable>();

		// internal properties for accessing Interactors and Interactables (used by XR Interaction Debugger)
		internal List<BaseInteractor> interactors { get { return m_Interactors; } }
		internal List<BaseInteractable> interactables { get { return m_Interactables; } }

		// map of all registered objects to test for colliding
		Dictionary<Collider, BaseInteractable> m_ColliderToInteractableMap = new Dictionary<Collider, BaseInteractable>();

		// reusable list of interactables for retrieving hover targets
		List<BaseInteractable> m_HoverTargetList = new List<BaseInteractable>();

		// reusable list of valid targets for and interactor
		List<BaseInteractable> m_InteractorValidTargets = new List<BaseInteractable>();

		protected virtual void OnEnable()
		{
			Application.onBeforeRender += OnBeforeRender;
		}

		protected virtual void OnDisable()
		{
			Application.onBeforeRender -= OnBeforeRender;
		}

		[BeforeRenderOrder(InteractionUpdateOrder.k_BeforeRenderOrder)]
		void OnBeforeRender()
		{
			ProcessInteractors(InteractionUpdateOrder.UpdatePhase.OnBeforeRender);
			ProcessInteractables(InteractionUpdateOrder.UpdatePhase.OnBeforeRender);
		}

		void ProcessInteractors(InteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (var interactor in m_Interactors)
			{
				interactor.ProcessInteractor(updatePhase);
			}
		}
		   
		void ProcessInteractables(InteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (var interactable in m_Interactables)
			{
				interactable.ProcessInteractable(updatePhase);
			}
		}

		private void LateUpdate()
		{
			ProcessInteractors(InteractionUpdateOrder.UpdatePhase.Late);
			ProcessInteractables(InteractionUpdateOrder.UpdatePhase.Late);
		}

		private void FixedUpdate()
		{
			ProcessInteractors(InteractionUpdateOrder.UpdatePhase.Fixed);
			ProcessInteractables(InteractionUpdateOrder.UpdatePhase.Fixed);
		}

		void Update()
		{
			ProcessInteractors(InteractionUpdateOrder.UpdatePhase.Dynamic);

			foreach (var interactor in m_Interactors)
			{
				GetValidTargets(interactor, m_InteractorValidTargets);

				ClearInteractorHover(interactor, m_InteractorValidTargets);
				InteractorHoverValidTargets(interactor, m_InteractorValidTargets);
			}

			ProcessInteractables(InteractionUpdateOrder.UpdatePhase.Dynamic);
		}

		internal void RegisterInteractor(BaseInteractor interactor)
		{
			if (!m_Interactors.Contains(interactor))
			{
				m_Interactors.Add(interactor);
			}
		}

		internal void UnregisterInteractor(BaseInteractor interactor)
		{
			if (m_Interactors.Contains(interactor))
			{                           
				ClearInteractorHover(interactor, null);
				m_Interactors.Remove(interactor);
			}
		}

		internal void RegisterInteractable(BaseInteractable interactable)
		{
			if (!m_Interactables.Contains(interactable))
			{
				m_Interactables.Add(interactable);
				
				foreach (var collider in interactable.colliders)
				{
					if (collider != null && !m_ColliderToInteractableMap.ContainsKey(collider))
					{
						m_ColliderToInteractableMap.Add(collider, interactable);
					}
				}
			}
		}

		internal void UnregisterInteractable(BaseInteractable interactable)
		{
			if (m_Interactables.Contains(interactable))
			{
				m_Interactables.Remove(interactable);

				foreach (var collider in interactable.colliders)
				{
					if (collider != null && m_ColliderToInteractableMap.ContainsKey(collider))
					{
						m_ColliderToInteractableMap.Remove(collider);
					}
				}
			}
		}

		internal BaseInteractable TryGetInteractableForCollider(Collider collider)
		{
			return collider != null && m_ColliderToInteractableMap.TryGetValue(collider, out BaseInteractable interactable)
				? interactable
				: null;
		}

		internal List<BaseInteractable> GetValidTargets(BaseInteractor interactor, List<BaseInteractable> validTargets)
		{
			interactor.GetValidTargets(validTargets);

			// Remove interactables that are not being handled by this manager.
			for (int i = validTargets.Count - 1; i >= 0; --i)
			{
				if (!m_Interactables.Contains(validTargets[i]))
				{
					validTargets.RemoveAt(i);
				}
			}
			return validTargets;
		}

		void ClearInteractorHover(BaseInteractor interactor, List<BaseInteractable> validTargets)
		{
			interactor.GetHoverTargets(m_HoverTargetList);
			for (int i = 0; i < m_HoverTargetList.Count; i++)
			{
				var target = m_HoverTargetList[i];
				if (!interactor.allowHover
					|| !interactor.CanHover(target)
					|| !target.IsHoverableBy(interactor)
					|| validTargets == null
					|| !validTargets.Contains(target))
				{
					HoverExit(interactor, target);
				}
			}
		}

		void HoverEnter(BaseInteractor interactor, BaseInteractable interactable)
		{
			interactor.OnHoverEnter(interactable);
			interactable.OnHoverEnter(interactor);
		}

		void HoverExit(BaseInteractor interactor, BaseInteractable interactable)
		{
			interactor.OnHoverExit(interactable);
			interactable.OnHoverExit(interactor);
		}

		void InteractorHoverValidTargets(BaseInteractor interactor, List<BaseInteractable> validTargets)
		{
			if (interactor.allowHover)
			{
				for (int i = 0; i < validTargets.Count && interactor.allowHover; ++i)
				{
					interactor.GetHoverTargets(m_HoverTargetList);
					if (interactor.CanHover(validTargets[i])
						&& validTargets[i].IsHoverableBy(interactor)
						&& !m_HoverTargetList.Contains(validTargets[i]))
					{
						HoverEnter(interactor, validTargets[i]);
					}
				}
			}
		}
	}
}