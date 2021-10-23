using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.Interaction.Toolkit
{
	/// <summary>
	/// UnityEvent that responds to changes of hover, selection, and activation by this interactable.
	/// </summary>
	[Serializable]
	public class XRInteractableEvent : UnityEvent<BaseInteractor> { }

	/// <summary>
	/// Abstract base class from which all interactable behaviours derive.
	/// This class hooks into the interaction system (via XRInteractionManager) and provides base virtual methods for handling
	/// hover and selection.
	/// </summary>
	[SelectionBase]
	[DefaultExecutionOrder(InteractionUpdateOrder.k_Interactables)]
	public abstract class BaseInteractable : MonoBehaviour
	{
		/// <summary>Type of movement for an interactable</summary>
		public enum MovementType
		{
			/// <summary>In VelocityTracking mode, the Rigid Body associated with the will have velocity and angular velocity added to it such that the interactable attach point will follow the interactor attach point
			/// as this is applying forces to the RigidBody, this will appear to be a slight distance behind the visual representation of the Interactor / Controller</summary>
			VelocityTracking,
			/// <summary>In Kinematic mode the Rigid Body associated with the interactable will be moved such that the interactable attach point will match the interactor attach point
			/// as this is updating the RigidBody, this will appear a frame behind the visual representation of the Interactor / Controller </summary>
			Kinematic,
			/// <summary>In Instantaneous Mode the interactable's transform is updated such that the interactable attach point will match the interactor's attach point.
			/// as this is updating the transform directly, any rigid body attached to the GameObject that the interactable component is on will be disabled while being interacted with so
			/// that any motion will not "judder" due to the rigid body interfering with motion.</summary>
			Instantaneous,
		};

		[SerializeField, Tooltip("Manager to handle all interaction management (will find one if empty).")]
		InteractionManager m_InteractionManager;
		/// <summary>Gets or sets Interaction Manager.</summary>
		public InteractionManager interactionManager
		{
			get { return m_InteractionManager; }
			set
			{
				m_InteractionManager = value;
				RegisterWithInteractionMananger();
			}
		}

		[SerializeField, Tooltip("Colliders to use for interaction (if empty, will use any child colliders).")]
		List<Collider> m_Colliders = new List<Collider>();
		/// <summary>Gets colliders to use for interaction with this interactable.</summary>
		public List<Collider> colliders { get { return m_Colliders; } }

		[SerializeField, Tooltip("Only interactors with this Layer Mask will interact with this interactable.")]
		LayerMask m_InteractionLayerMask = -1;
		/// <summary>Gets or sets the layer mask to use to filter interactors that can interact with this interactable.</summary>
		public LayerMask interactionLayerMask { get { return m_InteractionLayerMask; } set { m_InteractionLayerMask = value; } }

		[SerializeField]
		private Transform m_Target;
		public Transform target => m_Target;
		
		public int priority;

		List<BaseInteractor> m_HoveringInteractors = new List<BaseInteractor>();
		/// <summary>Gets the list of interactors that are hovering on this interactable; </summary>
		public List<BaseInteractor> hoveringInteractors { get { return m_HoveringInteractors; } }

		/// <summary>Gets whether this interactable is currently being hovered.</summary>
		public bool isHovered { get; private set; }

		InteractionManager m_RegisteredInteractionManager = null;

		[Header("Interactable Events")]

		[SerializeField, Tooltip("Called only when the first interactor begins hovering over this interactable.")]
		XRInteractableEvent m_OnFirstHoverEnter = new XRInteractableEvent();
		/// <summary>Gets or sets the event that is called only when the first interactor begins hovering over this interactable.</summary>
		public XRInteractableEvent onFirstHoverEnter { get { return m_OnFirstHoverEnter; } set { m_OnFirstHoverEnter = value; } }

		[SerializeField, Tooltip("Called every time when an interactor begins hovering this interactable.")]
		XRInteractableEvent m_OnHoverEnter = new XRInteractableEvent();
		/// <summary>Gets or sets the event that is called every time when an interactor begins hovering over this interactable.</summary>
		public XRInteractableEvent onHoverEnter { get { return m_OnHoverEnter; } set { m_OnHoverEnter = value; } }

		[SerializeField, Tooltip("Called every time when an interactor stops hovering over this interactable.")]
		XRInteractableEvent m_OnHoverExit = new XRInteractableEvent();
		/// <summary>Gets or sets the event that is called every time when an interactor stops hovering over this interactable.</summary>
		public XRInteractableEvent onHoverExit { get { return m_OnHoverExit; } set { m_OnHoverExit = value; } }

		[SerializeField, Tooltip("Called only when the last interactor stops hovering over this interactable.")]
		XRInteractableEvent m_OnLastHoverExit = new XRInteractableEvent();
		/// <summary>Gets or sets the event that is called only when the last interactor stops hovering over this interactable.</summary>
		public XRInteractableEvent onLastHoverExit { get { return m_OnLastHoverExit; } set { m_OnLastHoverExit = value; } }

		protected virtual void Reset()
		{
			FindCreateInteractionManager();
		}

		protected virtual void Awake()
		{
			// if we have no colliders, add children colliders
			if (m_Colliders.Count <= 0)
			{
				m_Colliders = new List<Collider>(GetComponentsInChildren<Collider>());
			}

			// setup interaction manager
			if (!m_InteractionManager)
			{
				m_InteractionManager = FindObjectOfType<InteractionManager>();
			}

			if (m_InteractionManager)
			{
				RegisterWithInteractionMananger();
			}
			else
			{
				Debug.LogWarning("Could not find InteractionManager.", this);
			}

			if (m_Target == null)
			{
				m_Target = transform;
			}

			InteractionManager.Initialized += InteractionManager_Initialized;
			InteractionManager.Terminated += InteractionManager_Terminated;
		}

		void FindCreateInteractionManager()
		{
			if (m_InteractionManager == null)
			{
				m_InteractionManager = Object.FindObjectOfType<InteractionManager>();
				if (m_InteractionManager == null)
				{
					var interactionManagerGO = new GameObject("Interaction Manager", typeof(InteractionManager));
					if (interactionManagerGO)
						m_InteractionManager = interactionManagerGO.GetComponent<InteractionManager>();
				}
			}
		}

		void RegisterWithInteractionMananger()
		{
			if (m_InteractionManager != m_RegisteredInteractionManager)
			{
				if (m_RegisteredInteractionManager != null)
				{
					m_RegisteredInteractionManager.UnregisterInteractable(this);
					m_RegisteredInteractionManager = null;
				}

				if (m_InteractionManager)
				{
					m_InteractionManager.RegisterInteractable(this);
					m_RegisteredInteractionManager = m_InteractionManager;
				}
			}
		}

		private void InteractionManager_Initialized(InteractionManager manager)
		{
			if (m_RegisteredInteractionManager == null)
			{
				m_InteractionManager = manager;
				RegisterWithInteractionMananger();
			}
		}

		private void InteractionManager_Terminated(InteractionManager manager)
		{
			if (m_RegisteredInteractionManager == manager)
			{
				m_RegisteredInteractionManager = null;
			}
		}

		void OnDestroy()
		{
			if (m_RegisteredInteractionManager)
			{
				m_InteractionManager.UnregisterInteractable(this);
			}

			InteractionManager.Initialized -= InteractionManager_Initialized;
			InteractionManager.Terminated -= InteractionManager_Terminated;
		}

		/// <summary>
		/// Calculates distance squared to interactor (based on colliders).
		/// </summary>
		/// <param name="interactor">Interactor to calculate distance against.</param>
		/// <returns>Minimum distance between the interactor and this interactable's colliders.</returns>
		public float GetDistanceSqrToInteractor(BaseInteractor interactor)
		{
			if (!interactor)
				return float.MaxValue;

			float minDistanceSqr = float.MaxValue;
			foreach (var col in m_Colliders)
			{
				var offset = (interactor.attachTransform.position - col.transform.position);
				minDistanceSqr = Mathf.Min(offset.sqrMagnitude, minDistanceSqr);
			}
			return minDistanceSqr;
		}

		bool IsOnValidLayerMask(BaseInteractor interactor)
		{
			return (interactionLayerMask & interactor.interactionLayerMask) != 0;
		}

		/// <summary>
		/// Determines if this interactable can be hovered by a given interactor.
		/// </summary>
		/// <param name="interactor">Interactor to check for a valid hover state with.</param>
		/// <returns>True if hovering is valid this frame, False if not.</returns>
		public virtual bool IsHoverableBy(BaseInteractor interactor) { return IsOnValidLayerMask(interactor); }

		/// <summary>
		/// Determines if this interactable can be selected by a given interactor.
		/// </summary>
		/// <param name="interactor">Interactor to check for a valid selection with.</param>
		/// <returns>True if selection is valid this frame, False if not.</returns>
		public virtual bool IsSelectableBy(BaseInteractor interactor) { return IsOnValidLayerMask(interactor); }

		/// <summary>This method is called by the interaction manager 
		/// when the interactor first initiates hovering over an interactable.</summary>
		/// <param name="interactor">Interactor that is initiating the hover.</param>
		protected internal virtual void OnHoverEnter(BaseInteractor interactor) 
		{
			isHovered = true;
			m_HoveringInteractors.Add(interactor);

			if (m_HoveringInteractors.Count == 1)
				m_OnFirstHoverEnter?.Invoke(interactor);

			m_OnHoverEnter?.Invoke(interactor);
		}

		/// <summary>This method is called by the interaction manager 
		/// when the interactor ends hovering over an interactable.</summary>
		/// <param name="interactor">Interactor that is ending the hover.</param>
		protected internal virtual void OnHoverExit(BaseInteractor interactor) 
		{
			isHovered = false;
			if (m_HoveringInteractors.Contains(interactor))
				m_HoveringInteractors.Remove(interactor);

			if (m_HoveringInteractors.Count == 0)
				m_OnLastHoverExit?.Invoke(interactor);

			m_OnHoverExit?.Invoke(interactor);
		}

		/// <summary>
		/// This method is called by the interaction manager to update the interactable. 
		/// Please see the interaction manager documentation for more details on update order
		/// </summary>        
		public virtual void ProcessInteractable(InteractionUpdateOrder.UpdatePhase updatePhase)
		{
			return;
		}
		
		public virtual void CreateTarget()
		{
			if (m_Target == transform)
			{
				var obj = new GameObject("Target");
				obj.transform.SetParent(transform, false);
				m_Target = obj.transform;
			}
		}
	}
}
