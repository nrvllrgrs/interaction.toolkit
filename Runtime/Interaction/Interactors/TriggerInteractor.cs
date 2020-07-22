using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for directly interacting with interactables that are touching.  This is handled via trigger volumes
    /// that update the current set of valid targets for this interactor.  This component must have a collision volume that is 
    /// set to be a trigger to work.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Interaction/Interactors/Trigger Interactor")]
	public class TriggerInteractor : BaseInteractor
	{
        protected override List<BaseInteractable> ValidTargets { get { return m_ValidTargets; } }

        // reusable list of valid targets
        List<BaseInteractable> m_ValidTargets = new List<BaseInteractable>();

        // reusable map of interactables to their distance squared from this interactor (used for sort)
        Dictionary<BaseInteractable, float> m_InteractableDistanceSqrMap = new Dictionary<BaseInteractable, float>();

        // Sort comparison function used by GetValidTargets
        Comparison<BaseInteractable> m_InteractableSortComparison;
        int InteractableSortComparison(BaseInteractable x, BaseInteractable y)
        {
            float xDistance = m_InteractableDistanceSqrMap[x];
            float yDistance = m_InteractableDistanceSqrMap[y];
            if (xDistance > yDistance)
                return 1;
            if (xDistance < yDistance)
                return -1;
            else
                return 0;
        }

        protected override void Awake()
        {
            base.Awake();

            m_InteractableSortComparison = InteractableSortComparison;
            if (!GetComponents<Collider>().Any(x => x.isTrigger))
                Debug.LogWarning("Direct Interactor does not have required Collider set as a trigger.");
        }

        protected void OnTriggerEnter(Collider col)
        {
        	var interactable = interactionManager.TryGetInteractableForCollider(col);
            if (interactable && !m_ValidTargets.Contains(interactable))
                m_ValidTargets.Add(interactable);
        }

        protected void OnTriggerExit(Collider col)
        {
            var interactable = interactionManager.TryGetInteractableForCollider(col);
            if (interactable && m_ValidTargets.Contains(interactable))
                m_ValidTargets.Remove(interactable);
        }

        /// <summary>
        /// Retrieve the list of interactables that this interactor could possibly interact with this frame.
        /// This list is sorted by priority (in this case distance).
        /// </summary>
        /// <param name="validTargets">Populated List of interactables that are valid for selection or hover.</param>
        public override void GetValidTargets(List<BaseInteractable> validTargets)
		{
            validTargets.Clear();
            m_InteractableDistanceSqrMap.Clear();

            // Calculate distance squared to interactor's attach transform and add to validTargets (which is sorted before returning)
            foreach (var interactable in m_ValidTargets)
            {
                m_InteractableDistanceSqrMap[interactable] = interactable.GetDistanceSqrToInteractor(this);
                validTargets.Add(interactable);
            }

            validTargets.Sort(m_InteractableSortComparison);
        }

        /// <summary>Determines if the interactable is valid for hover this frame.</summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns><c>true</c> if the interactable can be hovered over this frame.</returns>
        public override bool CanHover(BaseInteractable interactable)
        {
            return base.CanHover(interactable) && (selectTarget == null || selectTarget == interactable);
        }

        /// <summary>Determines if the interactable is valid for selection this frame.</summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns><c>true</c> if the interactable can be selected this frame.</returns>
		public override bool CanSelect(BaseInteractable interactable)
        {
            return base.CanSelect(interactable) && (selectTarget == null || selectTarget == interactable);
        }
	}
}