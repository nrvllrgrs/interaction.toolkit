namespace UnityEngine.Interaction.Toolkit
{
	/// <summary>
	/// This is the simplest version of an Interactable object.
	/// It simply provides a public implementation of the BaseInteractable. 
	/// It is intended to be used as a way to respond to OnHoverEnter/Exit and OnSelectEnter/Exit events with no underlying interaction behaviour.
	/// </summary>
	[SelectionBase]
	[DisallowMultipleComponent]
	[AddComponentMenu("Interaction/Interactables/Simple Interactable")]
	public class SimpleInteractable : BaseInteractable
	{ }
}
