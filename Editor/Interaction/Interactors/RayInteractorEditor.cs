using UnityEngine;
using UnityEngine.Interaction.Toolkit;

namespace UnityEditor.Interaction.Toolkit
{
	[CustomEditor(typeof(RayInteractor))]
	[CanEditMultipleObjects]
	internal class RayInteractorEditor : Editor
	{
		SerializedProperty m_InteractionManager;
		SerializedProperty m_InteractionLayerMask;
		SerializedProperty m_AttachTransform;

		SerializedProperty m_MaxRaycastDistance;
		SerializedProperty m_HitDetectionType;
		SerializedProperty m_SphereCastRadius;        
		SerializedProperty m_RaycastMask;
		SerializedProperty m_RaycastTriggerInteraction;

		SerializedProperty m_LineType;

		SerializedProperty m_OnHoverEnter;
		SerializedProperty m_OnHoverExit;

		bool m_ShowInteractorEvents;

		static class Tooltips
		{
			public static readonly GUIContent interactionManager = new GUIContent("Interaction Manager", "Manager to handle all interaction management (will find one if empty).");
			public static readonly GUIContent interactionLayerMask = new GUIContent("Interaction Layer Mask", "Only interactables with this Layer Mask will respond to this interactor.");
			public static readonly GUIContent attachTransform = new GUIContent("Attach Transform", "Attach Transform to use for this Interactor.  Will create empty GameObject if none set.");
			public static readonly GUIContent maxRaycastDistance = new GUIContent("Max Raycast Distance", "Max distance of ray cast. Increase this value will let you reach further.");
			public static readonly GUIContent sphereCastRadius = new GUIContent("Sphere Cast Radius", "Radius of this Interactor's ray, used for spherecasting.");
			public static readonly GUIContent raycastMask = new GUIContent("Raycast Mask", "Layer mask used for limiting raycast targets.");
			public static readonly GUIContent raycastTriggerInteraction = new GUIContent("Raycast Trigger Interaction", "Type of interaction with trigger colliders via raycast.");
			public static readonly GUIContent lineType = new GUIContent("Line Type", "Line type of the ray cast.");
			public static readonly GUIContent hitDetectionType = new GUIContent("Hit Detection Type", "The type of hit detection used to hit interactable objects.");
			public static readonly string startingInteractableWarning = "A Starting Selected Interactable will be instantly deselected unless the Interactor's Toggle Select Mode is set to 'Toggle' or 'Sticky'.";
		}

		void OnEnable()
		{
			m_InteractionManager = serializedObject.FindProperty("m_InteractionManager");
			m_InteractionLayerMask = serializedObject.FindProperty("m_InteractionLayerMask");
			m_AttachTransform = serializedObject.FindProperty("m_AttachTransform");
			m_MaxRaycastDistance = serializedObject.FindProperty("m_MaxRaycastDistance");
			m_SphereCastRadius = serializedObject.FindProperty("m_SphereCastRadius");
			m_HitDetectionType = serializedObject.FindProperty("m_HitDetectionType");
			m_RaycastMask = serializedObject.FindProperty("m_RaycastMask");
			m_RaycastTriggerInteraction = serializedObject.FindProperty("m_RaycastTriggerInteraction");

			m_LineType = serializedObject.FindProperty("m_LineType");

			m_OnHoverEnter = serializedObject.FindProperty("m_OnHoverEnter");
			m_OnHoverExit = serializedObject.FindProperty("m_OnHoverExit");
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((RayInteractor)target), typeof(RayInteractor), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.PropertyField(m_InteractionManager, Tooltips.interactionManager);
			EditorGUILayout.PropertyField(m_InteractionLayerMask, Tooltips.interactionLayerMask);
			EditorGUILayout.PropertyField(m_AttachTransform, Tooltips.attachTransform);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_MaxRaycastDistance, Tooltips.maxRaycastDistance);
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_HitDetectionType, Tooltips.hitDetectionType);
			using (new EditorGUI.DisabledScope(m_HitDetectionType.enumValueIndex != (int)RayInteractor.HitDetectionType.SphereCast))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_SphereCastRadius, Tooltips.sphereCastRadius);
				EditorGUI.indentLevel--;
			}
		
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_RaycastMask, Tooltips.raycastMask);
			EditorGUILayout.PropertyField(m_RaycastTriggerInteraction, Tooltips.raycastTriggerInteraction);

			EditorGUILayout.Space();

			EditorGUILayout.Space();

			m_ShowInteractorEvents = EditorGUILayout.Foldout(m_ShowInteractorEvents, "Interactor Events");

			if (m_ShowInteractorEvents)
			{
				// UnityEvents have not yet supported Tooltips
				EditorGUILayout.PropertyField(m_OnHoverEnter);
				EditorGUILayout.PropertyField(m_OnHoverExit);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
