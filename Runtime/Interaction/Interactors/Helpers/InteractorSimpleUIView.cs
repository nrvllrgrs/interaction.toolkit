using System.Linq;
using TMPro;

namespace UnityEngine.Interaction.Toolkit.UI
{
	public class InteractorSimpleUIView : BaseInteractorUIView<InteractableSimpleUIModel>
	{
		#region Fields

		public ViewInfo[] items;

		#endregion

		#region Methods

		protected override void SetModel(InteractableSimpleUIModel model)
		{
			int modelLength = 0;
			if (model != null)
			{
				foreach (var item in model.items.Zip(items, (x, y) => new
				{
					model = x,
					view = y
				}))
				{
					item.view.textMesh.text = string.Format("<sprite name=\"{0}\"> {1}", item.model.sprite, item.model.text);
				}

				modelLength = model.items.Length;
			}

			for (int i = modelLength; i < items.Length; ++i)
			{
				items[i].textMesh.text = string.Empty;
			}
		}
		
		protected override void RefreshView()
		{ }

		#endregion

		#region Structures

		[System.Serializable]
		public struct ViewInfo
		{
			public TextMeshProUGUI textMesh;
		}

		#endregion
	}
}