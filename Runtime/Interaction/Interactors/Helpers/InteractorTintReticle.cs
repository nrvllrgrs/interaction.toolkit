using UnityEngine.UI;

namespace UnityEngine.Interaction.Toolkit.UI
{
	public class InteractorTintReticle : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private BaseInteractor m_Interactor;

		[SerializeField]
		private Graphic m_Graphic;

		[SerializeField]
		private Color m_NormalColor = Color.white;

		[SerializeField]
		private Color m_HoverColor = Color.red;

		#endregion

		#region Methods

		private void OnEnable()
		{
			if (m_Interactor != null)
			{
				m_Interactor.onHoverEnter.AddListener(HoverEnter);
				m_Interactor.onHoverExit.AddListener(HoverExit);
			}
		}

		private void OnDisable()
		{
			if (m_Interactor != null)
			{
				m_Interactor.onHoverEnter.RemoveListener(HoverEnter);
				m_Interactor.onHoverExit.RemoveListener(HoverExit);
			}
		}

		private void HoverEnter(BaseInteractable interactable)
		{
			m_Graphic.color = m_HoverColor;
		}

		private void HoverExit(BaseInteractable interactable)
		{
			m_Graphic.color = m_NormalColor;
		}

		#endregion
	}
}