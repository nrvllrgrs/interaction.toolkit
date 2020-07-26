using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace UnityEngine.Interaction.Toolkit
{
	/// <summary>
	/// UnityEvent that responds to changes of hover and selection by this interactor.
	/// </summary>
	[Serializable]    
	public class XRInteractorEvent : UnityEvent<BaseInteractable> { }

	/// <summary>
	/// Abstract base class from which all interactable behaviours derive.
	/// This class hooks into the interaction system (via XRInteractionManager) and provides base virtual methods for handling
	/// hover and selection.
	/// </summary>
	[SelectionBase]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(InteractionUpdateOrder.k_Interactors)]
	public abstract class BaseInteractor : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		[Tooltip("The Interaction Manager that this interactor will communicate with.")]
		private InteractionManager m_InteractionManager;

		[SerializeField]
		[Tooltip("Only interactables with this layer mask will respond to this interactor.")]
		private LayerMask m_InteractionLayerMask = -1;

		[SerializeField]
		private Transform m_AttachTransform;

		public Transform attachTransform => m_AttachTransform ?? transform;

		/// <summary>
		/// Target interactables that are currently being hovered over. (may by empty)
		/// </summary>
		protected List<BaseInteractable> m_HoverTargets = new List<BaseInteractable>();

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets Interaction Manager.
		/// </summary>
		public InteractionManager interactionManager
		{
			get { return m_InteractionManager; }
			set
			{
				m_InteractionManager = value;
				RegisterWithInteractionMananger();
			}
		}

		/// <summary>
		/// Gets or sets interaction layer mask.  Only interactables with this layer mask will respond to this interactor.
		/// </summary>
		public LayerMask interactionLayerMask { get => m_InteractionLayerMask; set => m_InteractionLayerMask = value; }

		public bool allowHover { get; set; } = true;

		/// <summary>
		/// Gets a list of targets that can be selected.
		/// </summary>
		protected abstract List<BaseInteractable> ValidTargets { get; }

		#endregion

		#region Methods

		protected virtual void Awake()
		{
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
		}

		protected virtual void OnEnable()
		{
			if (m_RequiresRegistration)
			{
				FindCreateInteractionManager();
				if (m_RegisteredInteractionManager)
				{
					m_RegisteredInteractionManager.RegisterInteractor(this);
				}
				m_RequiresRegistration = false;
			}
		}

		protected virtual void OnDisable()
		{
			if (m_RegisteredInteractionManager)
			{
				m_RegisteredInteractionManager.UnregisterInteractor(this);
			}

			m_RequiresRegistration = true;
		}

		private void OnDestroy()
		{
			if (m_RegisteredInteractionManager)
			{
				m_RegisteredInteractionManager.UnregisterInteractor(this);
			}
		}

		protected virtual void Reset()
		{
			FindCreateInteractionManager();
		}

		/// <summary>
		/// Retrieve the list of interactables that this interactor could possibly interact with this frame.
		/// </summary>
		/// <param name="validTargets">Populated List of interactables that are valid for selection or hover.</param>
		public abstract void GetValidTargets(List<BaseInteractable> validTargets);

		/// <summary>Determines if the interactable is valid for hover this frame.</summary>
		/// <param name="interactable">Interactable to check.</param>
		/// <returns><c>true</c> if the interactable can be hovered over this frame.</returns>
		public virtual bool CanHover(BaseInteractable interactable) { return allowHover && IsOnValidLayerMask(interactable); }

		#endregion

		InteractionManager m_RegisteredInteractionManager = null;

		[SerializeField, Tooltip("Called when this interactor begins hovering an interactable.")]
		XRInteractorEvent m_OnHoverEnter = new XRInteractorEvent();
		/// <summary>Gets or sets the event that is called when this interactor begins hovering over an interactable.</summary>
		public XRInteractorEvent onHoverEnter { get { return m_OnHoverEnter; } set { m_OnHoverEnter = value; } }

		[SerializeField, Tooltip("Called when this interactor stops the hovering an interactable.")]
		XRInteractorEvent m_OnHoverExit = new XRInteractorEvent();
		/// <summary>Gets or sets the event that is called when this interactor stops hovering over an interactable.</summary>
		public XRInteractorEvent onHoverExit { get { return m_OnHoverExit; } set { m_OnHoverExit = value; } }

		bool m_RequiresRegistration = true;

		public void GetHoverTargets(List<BaseInteractable> hoverTargets)
		{
			hoverTargets.Clear();
			foreach (var target in m_HoverTargets)
			{
				hoverTargets.Add(target);
			}
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
				if (m_RegisteredInteractionManager != null && m_InteractionManager != m_RegisteredInteractionManager)
				{
					m_RegisteredInteractionManager.UnregisterInteractor(this);
					m_RegisteredInteractionManager = null;
				}
				if (m_InteractionManager)
				{
					m_InteractionManager.RegisterInteractor(this);
					m_RegisteredInteractionManager = m_InteractionManager;
				}
			}

			if (m_RegisteredInteractionManager != null)
				m_RequiresRegistration = false;
		}

		internal void ClearHoverTargets()
		{
			m_HoverTargets.Clear();
		}

		bool IsOnValidLayerMask(BaseInteractable interactable)
		{
			return (interactionLayerMask & interactable.interactionLayerMask) != 0;
		}

		/// <summary>This method is called by the interaction manager 
		/// when the interactor first initiates hovering over an interactable.</summary>
		/// <param name="interactable">Interactable that is being hovered over.</param>
		protected internal virtual void OnHoverEnter(BaseInteractable interactable)
		{
			m_HoverTargets.Add(interactable);
			m_OnHoverEnter?.Invoke(interactable);
		}

		/// <summary>This method is called by the interaction manager 
		/// when the interactor ends hovering over an interactable.</summary>
		/// <param name="interactable">Interactable that is no longer hovered over.</param>
		protected internal virtual void OnHoverExit(BaseInteractable interactable)
		{
			Debug.Assert(m_HoverTargets.Contains(interactable));
			m_HoverTargets.Remove(interactable);
			m_OnHoverExit?.Invoke(interactable);
		}

		/// <summary>
		/// This method is called by the interaction manager to update the interactor. 
		/// Please see the interaction manager documentation for more details on update order
		/// </summary>
		public virtual void ProcessInteractor(InteractionUpdateOrder.UpdatePhase updatePhase)
		{ }
	}
}