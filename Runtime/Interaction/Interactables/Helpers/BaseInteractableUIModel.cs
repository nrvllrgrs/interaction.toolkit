using UnityEngine;

namespace UnityEngine.Interaction.Toolkit.UI
{
	public abstract class BaseInteractableUIModel : MonoBehaviour
	{
		public event System.EventHandler onChanged;
		
		protected void InvokeChanged()
		{
			onChanged?.Invoke(this, System.EventArgs.Empty);
		}
	}
}
