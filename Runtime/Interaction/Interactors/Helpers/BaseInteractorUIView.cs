namespace UnityEngine.Interaction.Toolkit.UI
{
	public abstract class BaseInteractorUIView<T> : MonoBehaviour
		where T : BaseInteractableUIModel
	{
		#region Fields

		[SerializeField]
		protected BaseInteractor m_Interactor;

		[SerializeField]
		protected bool m_TrackTarget;

		[Header("UI Settings")]

		[SerializeField]
		protected float m_TransitionTime;

		protected RectTransform m_RectTransform;
		protected Canvas m_Canvas;
		protected BaseInteractable m_HoverInteractable;
		protected T m_HoverModel;

		#endregion

		#region Properties

		public BaseInteractable hoverInteractable => m_HoverInteractable;
		public BaseInteractableUIModel hoverModel => m_HoverModel;
		
		protected RectTransform rectTransform => this.GetComponent(ref m_RectTransform);
		protected Canvas canvas => this.GetComponentInParent(ref m_Canvas);

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			if (m_Interactor != null)
			{
				m_Interactor.onHoverEnter.AddListener(HoverEnter);
				m_Interactor.onHoverExit.AddListener(HoverExit);
			}

			SetModel(null);
		}

		protected virtual void OnDisable()
		{
			if (m_Interactor != null)
			{
				m_Interactor.onHoverEnter.RemoveListener(HoverEnter);
				m_Interactor.onHoverExit.RemoveListener(HoverExit);
			}
		}

		protected virtual void HoverEnter(BaseInteractable interactable)
		{
			var model = interactable.GetComponent<T>();
			if (model == null)
				return;

			m_HoverInteractable = interactable;
			
			m_HoverModel = model;
			m_HoverModel.onChanged += ModelChanged;
			SetModel(model);
		}

		protected virtual void HoverExit(BaseInteractable interactable)
		{
			// Cleared interactable
			if (m_HoverInteractable == interactable)
			{
				m_HoverInteractable = null;
				
				m_HoverModel.onChanged -= ModelChanged;
				m_HoverModel = null;
				SetModel(null);
			}
		}
		
		private void ModelChanged(object sender, System.EventArgs e)
		{
			RefreshView();
		}

		protected virtual void RefreshView()
		{
			SetModel(m_HoverModel);
		}

		protected abstract void SetModel(T model);

		protected virtual void Update()
		{
			if (!m_TrackTarget || m_HoverInteractable == null)
				return;

			// Move pivot of UI transform to the interactable pivot in canvas space
			rectTransform.anchoredPosition3D = canvas.WorldToCanvasPoint(m_HoverInteractable.transform.position);
		}

		#endregion
	}
}