using UnityEngine;
using UnityEngine.Interaction.Toolkit;

namespace UnityEditor.Interaction.Toolkit
{
	internal static class CreateUtils
    {
        static readonly string k_LineMaterial = "Default-Line.mat";

        static void CreateInteractionManager()
        {
            //if (!Object.FindObjectOfType<InteractionManager>())
            //{
            //    ObjectFactory.CreateGameObject("XR Interaction Manager", typeof(InteractionManager));
            //}
        }

        static GameObject CreateRayInteractorInternal(string gameObjectName)
        {
            var rayInteractableGo = ObjectFactory.CreateGameObject(gameObjectName,
                typeof(RayInteractor),
                typeof(LineRenderer));

            //SetupLineRenderer(rayInteractableGo.GetComponent<LineRenderer>());

            return rayInteractableGo;
        }

        [MenuItem("GameObject/XR/Ray Interactor", false, 10)]
        static void CreateRayInteractor()
        {
            CreateInteractionManager();
            CreateRayInteractorInternal("Ray Interactor");
        }

        [MenuItem("GameObject/XR/Direct Interactor", false, 10)]
        static void CreateDirectInteractor()
        {
            CreateInteractionManager();

            var directInteractableGo = ObjectFactory.CreateGameObject("Direct Interactor",
                typeof(SphereCollider),
                typeof(TriggerInteractor));
            var sphereCollider = directInteractableGo.GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 0.1f;
        }

        [MenuItem("GameObject/XR/Grab Interactable", false, 10)]
        static void CreateGrabInteractable()
        {
            CreateInteractionManager();

            var grabInteractableGo = ObjectFactory.CreateGameObject("Grab Interactable",
                typeof(GrabInteractable),
                typeof(SphereCollider));
            var sphereCollider = grabInteractableGo.GetComponent<SphereCollider>();
            sphereCollider.isTrigger = false;
            sphereCollider.radius = 0.1f;
        }
    }
}
