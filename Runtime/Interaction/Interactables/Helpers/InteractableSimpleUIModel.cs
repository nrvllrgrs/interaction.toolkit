namespace UnityEngine.Interaction.Toolkit.UI
{
	public class InteractableSimpleUIModel : BaseInteractableUIModel
	{
		#region Fields

		public ModelInfo[] items;

		#endregion

		#region Structures

		[System.Serializable]
		public struct ModelInfo
		{
			public string sprite;
			public string text;
		}

		#endregion
	}
}